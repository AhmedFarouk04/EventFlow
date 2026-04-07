using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;
using EventDrivenBookingPlatform.BuildingBlocks.Messaging.Outbox;
using System.Text.Json;

namespace EventDrivenBookingPlatform.Modules.Reservations.Infrastructure.Messaging;

public class ReservationEventPublisher
{
    private readonly IEventBus _eventBus;

    public ReservationEventPublisher(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task PublishAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        var type = Type.GetType(message.Type);
        if (type is null)
        {
            return;
        }

        var integrationEvent = JsonSerializer.Deserialize(message.Content, type) as IIntegrationEvent;
        if (integrationEvent is not null)
        {
            await _eventBus.PublishAsync(integrationEvent, cancellationToken);
        }
    }
}
