using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;

namespace EventDrivenBookingPlatform.Modules.Availability.Contracts.IntegrationEvents;

public record AvailabilityBlockedIntegrationEvent(
    Guid AvailabilityId,
    Guid ReservationId,
    Guid ServiceId,
    DateTime Date,
    int Quantity,
    DateTime OccurredOn) : IIntegrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreationDate { get; init; } = DateTime.UtcNow;
}
