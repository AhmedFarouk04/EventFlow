namespace EventDrivenBookingPlatform.BuildingBlocks.SharedKernel;

public abstract class AggregateRoot : BaseEntity
{
    public int Version { get; protected set; }
}