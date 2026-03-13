namespace EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;

public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTime CreationDate { get; }
}