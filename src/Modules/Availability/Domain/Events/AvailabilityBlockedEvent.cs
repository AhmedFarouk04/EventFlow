using EventDrivenBookingPlatform.BuildingBlocks.SharedKernel;

namespace EventDrivenBookingPlatform.Modules.Availability.Domain.Events;

public record AvailabilityBlockedEvent(Guid SlotId, Guid ServiceId, DateTime Date, int Quantity) : DomainEvent;
