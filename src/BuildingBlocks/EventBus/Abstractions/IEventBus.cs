namespace EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;

public interface IEventBus
{
    void Publish(IIntegrationEvent @event);

    void Subscribe<T, TH>()
        where T : IIntegrationEvent
        where TH : IIntegrationEventHandler<T>;

    void Unsubscribe<T, TH>()
        where TH : IIntegrationEventHandler<T>
        where T : IIntegrationEvent;
}