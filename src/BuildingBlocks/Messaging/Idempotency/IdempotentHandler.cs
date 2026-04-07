using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;

namespace EventDrivenBookingPlatform.BuildingBlocks.Messaging.Idempotency;

public abstract class IdempotentHandler<TIntegrationEvent> : IIntegrationEventHandler<TIntegrationEvent>
    where TIntegrationEvent : IIntegrationEvent
{
    private readonly IProcessedMessageStore _processedMessageStore;

    protected IdempotentHandler(IProcessedMessageStore processedMessageStore)
    {
        _processedMessageStore = processedMessageStore;
    }

    public async Task Handle(TIntegrationEvent @event)
    {
        var exists = await _processedMessageStore.ExistsAsync(@event.Id);
        if (exists)
        {
            return;
        }

        await HandleCoreAsync(@event);

        await _processedMessageStore.AddAsync(new ProcessedMessage
        {
            Id = Guid.NewGuid(),
            MessageId = @event.Id,
            Name = typeof(TIntegrationEvent).Name,
            ProcessedOn = DateTime.UtcNow
        });
    }

    protected abstract Task HandleCoreAsync(TIntegrationEvent @event);
}
