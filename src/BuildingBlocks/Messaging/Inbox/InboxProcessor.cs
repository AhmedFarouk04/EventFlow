using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventDrivenBookingPlatform.BuildingBlocks.Messaging.Inbox;

public class InboxProcessor : BackgroundService
{
    private readonly ILogger<InboxProcessor> _logger;

    public InboxProcessor(ILogger<InboxProcessor> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Placeholder: Logic to process Inbox messages safely will be injected here
            await Task.Delay(5000, stoppingToken);
        }
    }
}