namespace EventDrivenBookingPlatform.BuildingBlocks.Messaging.Inbox;

public interface IInboxStore
{
    Task AddAsync(InboxMessage message, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid messageId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InboxMessage>> GetUnprocessedMessagesAsync(int batchSize, CancellationToken cancellationToken = default);
    Task MarkAsProcessedAsync(Guid id, CancellationToken cancellationToken = default);
    Task MarkAsFailedAsync(Guid id, string error, CancellationToken cancellationToken = default);
}
