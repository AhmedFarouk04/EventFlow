using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;

namespace EventDrivenBookingPlatform.BuildingBlocks.Messaging.Idempotency;

public abstract class IdempotentHandler<TIntegrationEvent> : IIntegrationEventHandler<TIntegrationEvent>
    where TIntegrationEvent : IIntegrationEvent
{
    public async Task Handle(TIntegrationEvent @event)
    {
        if (await IsMessageProcessed(@event.Id))
        {
            return; // Already processed, idempotency applied. Skip safely.
        }

        await ProcessEvent(@event);
        await MarkMessageAsProcessed(@event.Id);
    }

    protected abstract Task<bool> IsMessageProcessed(Guid eventId);
    protected abstract Task ProcessEvent(TIntegrationEvent @event);
    protected abstract Task MarkMessageAsProcessed(Guid eventId);
}