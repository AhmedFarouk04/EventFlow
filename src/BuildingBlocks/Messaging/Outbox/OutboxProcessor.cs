using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EventDrivenBookingPlatform.BuildingBlocks.Messaging.Outbox;

public class OutboxProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(IServiceProvider serviceProvider, ILogger<OutboxProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var outboxStore = scope.ServiceProvider.GetRequiredService<IOutboxStore>();
                var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

                var messages = await outboxStore.GetUnprocessedMessagesAsync(stoppingToken);

                foreach (var message in messages)
                {
                    var eventType = Type.GetType(message.Type);
                    if (eventType != null)
                    {
                        var integrationEvent = JsonSerializer.Deserialize(message.Content, eventType) as IIntegrationEvent;
                        if (integrationEvent != null)
                        {
                            eventBus.Publish(integrationEvent);
                            await outboxStore.MarkAsProcessedAsync(message.Id, stoppingToken);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox messages.");
            }

            await Task.Delay(5000, stoppingToken); // Poll every 5 seconds
        }
    }
}