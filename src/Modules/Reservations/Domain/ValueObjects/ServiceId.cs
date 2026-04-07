using EventDrivenBookingPlatform.BuildingBlocks.SharedKernel;

namespace EventDrivenBookingPlatform.Modules.Reservations.Domain.ValueObjects;

public sealed class ServiceId : ValueObject
{
    public Guid Value { get; }

    public ServiceId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("ServiceId cannot be empty.", nameof(value));
        }

        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
