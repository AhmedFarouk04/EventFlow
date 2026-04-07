namespace EventDrivenBookingPlatform.BuildingBlocks.Messaging.Inbox;

public interface IInboxMessageDispatcher
{
    Task DispatchAsync(InboxMessage inboxMessage, CancellationToken cancellationToken = default);
}
