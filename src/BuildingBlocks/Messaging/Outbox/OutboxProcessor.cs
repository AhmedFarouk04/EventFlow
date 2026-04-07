using System.Text.Json;
using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventDrivenBookingPlatform.BuildingBlocks.Messaging.Outbox;

public class OutboxProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly OutboxProcessorOptions _options;

    public OutboxProcessor(
        IServiceProvider serviceProvider,
        IOptions<OutboxProcessorOptions> options,
        ILogger<OutboxProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox processor batch failed.");
            }

            await Task.Delay(TimeSpan.FromSeconds(_options.PollIntervalSeconds), stoppingToken);
        }
    }

    private async Task ProcessBatchAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var outboxStore = scope.ServiceProvider.GetRequiredService<IOutboxStore>();
        var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

        var messages = await outboxStore.GetUnprocessedMessagesAsync(_options.BatchSize, cancellationToken);
        if (messages.Count == 0)
        {
            return;
        }

        foreach (var message in messages)
        {
            try
            {
                var eventType = Type.GetType(message.Type);
                if (eventType is null)
                {
                    await outboxStore.MarkAsFailedAsync(message.Id, $"Cannot resolve event type '{message.Type}'.", cancellationToken);
                    continue;
                }

                var integrationEvent = JsonSerializer.Deserialize(message.Content, eventType) as IIntegrationEvent;
                if (integrationEvent is null)
                {
                    await outboxStore.MarkAsFailedAsync(message.Id, $"Cannot deserialize event payload for type '{message.Type}'.", cancellationToken);
                    continue;
                }

                await eventBus.PublishAsync(integrationEvent, cancellationToken);
                await outboxStore.MarkAsProcessedAsync(message.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                await outboxStore.MarkAsFailedAsync(message.Id, ex.Message, cancellationToken);
            }
        }
    }
}
