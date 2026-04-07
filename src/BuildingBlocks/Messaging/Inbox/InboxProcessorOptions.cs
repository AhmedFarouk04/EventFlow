namespace EventDrivenBookingPlatform.BuildingBlocks.Messaging.Inbox;

public class InboxProcessorOptions
{
    public int PollIntervalSeconds { get; set; } = 5;
    public int BatchSize { get; set; } = 50;
}
