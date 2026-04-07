using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;

namespace EventDrivenBookingPlatform.Modules.Pricing.Contracts.IntegrationEvents;

public record PriceCalculatedIntegrationEvent(
    Guid ReservationId,
    Guid ServiceId,
    int NumberOfDays,
    decimal TotalPrice,
    DateTime OccurredOn) : IIntegrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreationDate { get; init; } = DateTime.UtcNow;
}
