using EventDrivenBookingPlatform.BuildingBlocks.SharedKernel;

namespace EventDrivenBookingPlatform.Modules.Reservations.Domain.Events;

public record ReservationCreatedEvent(
    Guid ReservationId,
    Guid CustomerId,
    Guid ServiceId,
    DateTime StartDate,
    DateTime EndDate) : DomainEvent;
