using EventDrivenBookingPlatform.BuildingBlocks.SharedKernel;

namespace EventDrivenBookingPlatform.Modules.Reservations.Domain.ValueObjects;

public sealed class DateRange : ValueObject
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }

    public DateRange(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;

        if (startDate >= endDate)
        {
            throw new ArgumentException("Start date must be before end date.");
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
    }
}
