using EventDrivenBookingPlatform.BuildingBlocks.SharedKernel;

namespace EventDrivenBookingPlatform.Modules.Reservations.Domain.Events;

public record ReservationConfirmedEvent(
    Guid ReservationId,
    Guid CustomerId,
    Guid ServiceId) : DomainEvent;
