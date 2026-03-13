namespace EventDrivenBookingPlatform.BuildingBlocks.SharedKernel;

public abstract record DomainEvent(Guid EventId, DateTime OccurredOn)
{
    protected DomainEvent() : this(Guid.NewGuid(), DateTime.UtcNow)
    {
    }
}