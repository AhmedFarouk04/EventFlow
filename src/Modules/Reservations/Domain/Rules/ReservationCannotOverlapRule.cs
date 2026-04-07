using EventDrivenBookingPlatform.Modules.Reservations.Domain.ValueObjects;

namespace EventDrivenBookingPlatform.Modules.Reservations.Domain.Rules;

public sealed class ReservationCannotOverlapRule : IBusinessRule
{
    private readonly bool _hasOverlap;
    private readonly DateRange _requestedRange;

    public ReservationCannotOverlapRule(bool hasOverlap, DateRange requestedRange)
    {
        _hasOverlap = hasOverlap;
        _requestedRange = requestedRange;
    }

    public string Message => $"Reservation cannot overlap with another reservation in range {_requestedRange.StartDate:O} - {_requestedRange.EndDate:O}.";

    public bool IsBroken() => _hasOverlap;
}
