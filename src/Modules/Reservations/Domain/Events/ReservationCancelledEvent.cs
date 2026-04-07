using EventDrivenBookingPlatform.BuildingBlocks.SharedKernel;

namespace EventDrivenBookingPlatform.Modules.Reservations.Domain.Events;

public record ReservationCancelledEvent(
    Guid ReservationId,
    Guid CustomerId,
    Guid ServiceId,
    string Reason) : DomainEvent;
