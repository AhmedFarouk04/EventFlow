using System.Linq.Expressions;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.Aggregates;

namespace EventDrivenBookingPlatform.Modules.Reservations.Domain.Specifications;

public class OverlappingReservationSpecification
{
    private readonly string _customerEmail;
    private readonly DateTime _checkInDate;
    private readonly DateTime _checkOutDate;

    public OverlappingReservationSpecification(string customerEmail, DateTime checkInDate, DateTime checkOutDate)
    {
        _customerEmail = customerEmail;
        _checkInDate = checkInDate;
        _checkOutDate = checkOutDate;
    }

    public Expression<Func<Reservation, bool>> ToExpression()
    {
        return reservation =>
            reservation.CustomerInfo.Email == _customerEmail &&
            reservation.Status != ReservationStatus.Cancelled &&
            reservation.Status != ReservationStatus.Declined &&
            ((_checkInDate >= reservation.CheckInDate && _checkInDate < reservation.CheckOutDate) ||
             (_checkOutDate > reservation.CheckInDate && _checkOutDate <= reservation.CheckOutDate) ||
             (_checkInDate <= reservation.CheckInDate && _checkOutDate >= reservation.CheckOutDate));
    }
}