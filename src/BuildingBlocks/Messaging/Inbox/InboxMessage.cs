namespace EventDrivenBookingPlatform.BuildingBlocks.Messaging.Inbox;

public class InboxMessage
{
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime OccurredOn { get; set; }
    public DateTime? ProcessedOn { get; set; }
    public int RetryCount { get; set; }
    public DateTime? LastRetryOn { get; set; }
    public string? Error { get; set; }
}
