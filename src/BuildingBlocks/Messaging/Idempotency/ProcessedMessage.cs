namespace EventDrivenBookingPlatform.BuildingBlocks.Messaging.Idempotency;

public class ProcessedMessage
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime ProcessedOn { get; set; }
}