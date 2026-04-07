using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;

namespace EventDrivenBookingPlatform.Modules.Reservations.Contracts.IntegrationEvents;

public record ReservationConfirmedIntegrationEvent(
    Guid ReservationId,
    Guid CustomerId,
    Guid ServiceId,
    DateTime OccurredOn) : IIntegrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreationDate { get; init; } = DateTime.UtcNow;
}
