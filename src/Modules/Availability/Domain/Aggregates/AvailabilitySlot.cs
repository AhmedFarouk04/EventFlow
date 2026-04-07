using EventDrivenBookingPlatform.BuildingBlocks.SharedKernel;
using EventDrivenBookingPlatform.Modules.Availability.Domain.Events;

namespace EventDrivenBookingPlatform.Modules.Availability.Domain.Aggregates;

public class AvailabilitySlot : AggregateRoot
{
    public Guid ServiceId { get; private set; }
    public DateTime Date { get; private set; }
    public int Capacity { get; private set; }
    public int Reserved { get; private set; }

    public int Remaining => Capacity - Reserved;

    private AvailabilitySlot() { }

    public AvailabilitySlot(Guid serviceId, DateTime date, int capacity)
    {
        Id = Guid.NewGuid();
        ServiceId = serviceId;
        Date = date.Date;
        Capacity = capacity;
        Reserved = 0;
    }

    public Result Block(int quantity)
    {
        if (quantity <= 0)
        {
            return Result.Failure("Quantity must be greater than zero.");
        }

        if (Remaining < quantity)
        {
            return Result.Failure("Not enough availability.");
        }

        Reserved += quantity;
        AddDomainEvent(new AvailabilityBlockedEvent(Id, ServiceId, Date, quantity));

        return Result.Success();
    }

    public Result Release(int quantity)
    {
        if (quantity <= 0)
        {
            return Result.Failure("Quantity must be greater than zero.");
        }

        if (Reserved < quantity)
        {
            return Result.Failure("Cannot release more than reserved.");
        }

        Reserved -= quantity;
        return Result.Success();
    }
}
