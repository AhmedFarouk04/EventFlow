namespace EventDrivenBookingPlatform.BuildingBlocks.Messaging.Idempotency;

public interface IProcessedMessageStore
{
    Task<bool> ExistsAsync(Guid messageId, CancellationToken cancellationToken = default);
    Task AddAsync(ProcessedMessage message, CancellationToken cancellationToken = default);
}
