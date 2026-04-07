using EventDrivenBookingPlatform.Modules.Reservations.Domain.Aggregates;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.Events;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.ValueObjects;

namespace Reservations.UnitTests.Domain;

public class ReservationTests
{
    [Fact]
    public void Create_ShouldSucceed_ForFutureDateRange()
    {
        var result = Reservation.Create(
            ReservationId.New(),
            new CustomerId(Guid.NewGuid()),
            new ServiceId(Guid.NewGuid()),
            new DateRange(DateTime.UtcNow.Date.AddDays(1), DateTime.UtcNow.Date.AddDays(2)));

        Assert.True(result.IsSuccess);
        Assert.Equal(ReservationStatus.Pending, result.Value.Status);
        Assert.Contains(result.Value.DomainEvents, e => e is ReservationCreatedEvent);
    }

    [Fact]
    public void Confirm_ShouldRaiseDomainEvent()
    {
        var reservation = Reservation.Create(
            ReservationId.New(),
            new CustomerId(Guid.NewGuid()),
            new ServiceId(Guid.NewGuid()),
            new DateRange(DateTime.UtcNow.Date.AddDays(1), DateTime.UtcNow.Date.AddDays(2))).Value;

        var result = reservation.Confirm();

        Assert.True(result.IsSuccess);
        Assert.Equal(ReservationStatus.Confirmed, reservation.Status);
        Assert.Contains(reservation.DomainEvents, e => e is ReservationConfirmedEvent);
    }

    [Fact]
    public void Cancel_ShouldRaiseDomainEvent()
    {
        var reservation = Reservation.Create(
            ReservationId.New(),
            new CustomerId(Guid.NewGuid()),
            new ServiceId(Guid.NewGuid()),
            new DateRange(DateTime.UtcNow.Date.AddDays(1), DateTime.UtcNow.Date.AddDays(2))).Value;

        var result = reservation.Cancel("Changed plans");

        Assert.True(result.IsSuccess);
        Assert.Equal(ReservationStatus.Cancelled, reservation.Status);
        Assert.Contains(reservation.DomainEvents, e => e is ReservationCancelledEvent);
    }
}
