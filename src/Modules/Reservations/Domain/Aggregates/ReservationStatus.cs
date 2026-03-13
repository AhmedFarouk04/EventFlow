namespace EventDrivenBookingPlatform.Modules.Reservations.Domain.Aggregates;

public enum ReservationStatus
{
    Pending = 0,
    Confirmed = 1,
    CheckedIn = 2,
    CheckedOut = 3,
    Cancelled = 4,
    NoShow = 5,
    Refunded = 6,
    Declined = 7
}