namespace EventDrivenBookingPlatform.BuildingBlocks.Messaging.Outbox;

public class OutboxProcessorOptions
{
    public int PollIntervalSeconds { get; set; } = 5;
    public int BatchSize { get; set; } = 50;
}
