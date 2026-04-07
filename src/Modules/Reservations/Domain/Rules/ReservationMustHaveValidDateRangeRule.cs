using EventDrivenBookingPlatform.Modules.Reservations.Domain.ValueObjects;

namespace EventDrivenBookingPlatform.Modules.Reservations.Domain.Rules;

public sealed class ReservationMustHaveValidDateRangeRule : IBusinessRule
{
    private readonly DateRange _dateRange;

    public ReservationMustHaveValidDateRangeRule(DateRange dateRange)
    {
        _dateRange = dateRange;
    }

    public string Message => "Reservation must have a valid date range where StartDate is earlier than EndDate.";

    public bool IsBroken() => _dateRange.StartDate >= _dateRange.EndDate;
}
