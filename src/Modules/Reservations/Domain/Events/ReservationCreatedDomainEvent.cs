using EventDrivenBookingPlatform.BuildingBlocks.SharedKernel;

namespace EventDrivenBookingPlatform.Modules.Reservations.Domain.Events;

public record ReservationCreatedDomainEvent(
    Guid ReservationId,
    string CustomerEmail,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    decimal TotalPrice) : DomainEvent;