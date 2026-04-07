using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;

namespace EventDrivenBookingPlatform.Modules.Reservations.Contracts.IntegrationEvents;

public record ReservationCancelledIntegrationEvent(
    Guid ReservationId,
    Guid CustomerId,
    Guid ServiceId,
    string Reason,
    DateTime OccurredOn) : IIntegrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreationDate { get; init; } = DateTime.UtcNow;
}
