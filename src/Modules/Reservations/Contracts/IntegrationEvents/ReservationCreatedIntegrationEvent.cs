using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;

namespace EventDrivenBookingPlatform.Modules.Reservations.Contracts.IntegrationEvents;

public record ReservationCreatedIntegrationEvent(
    Guid ReservationId,
    string CustomerEmail,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    decimal TotalPrice) : IIntegrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreationDate { get; init; } = DateTime.UtcNow;
}