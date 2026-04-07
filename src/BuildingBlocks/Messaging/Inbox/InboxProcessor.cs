using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventDrivenBookingPlatform.BuildingBlocks.Messaging.Inbox;

public class InboxProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InboxProcessor> _logger;
    private readonly InboxProcessorOptions _options;

    public InboxProcessor(
        IServiceProvider serviceProvider,
        IOptions<InboxProcessorOptions> options,
        ILogger<InboxProcessor> logger)
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
                using var scope = _serviceProvider.CreateScope();
                var inboxStore = scope.ServiceProvider.GetRequiredService<IInboxStore>();
                var dispatcher = scope.ServiceProvider.GetRequiredService<IInboxMessageDispatcher>();

                var messages = await inboxStore.GetUnprocessedMessagesAsync(_options.BatchSize, stoppingToken);

                foreach (var message in messages)
                {
                    try
                    {
                        await dispatcher.DispatchAsync(message, stoppingToken);
                        await inboxStore.MarkAsProcessedAsync(message.Id, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        await inboxStore.MarkAsFailedAsync(message.Id, ex.Message, stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Inbox processor batch failed.");
            }

            await Task.Delay(TimeSpan.FromSeconds(_options.PollIntervalSeconds), stoppingToken);
        }
    }
}
