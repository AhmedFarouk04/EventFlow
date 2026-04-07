using EventDrivenBookingPlatform.Modules.Reservations.Domain.Aggregates;

namespace EventDrivenBookingPlatform.Modules.Reservations.Application.DTOs;

public record ReservationItemDto(Guid Id, string Name, int Quantity, decimal UnitPrice, decimal TotalPrice);

public record ReservationDto(
    Guid Id,
    Guid CustomerId,
    Guid ServiceId,
    DateTime StartDate,
    DateTime EndDate,
    int Status,
    IReadOnlyCollection<ReservationItemDto> Items)
{
    public static ReservationDto FromDomain(Reservation reservation)
    {
        return new ReservationDto(
            reservation.Id,
            reservation.CustomerId.Value,
            reservation.ServiceId.Value,
            reservation.DateRange.StartDate,
            reservation.DateRange.EndDate,
            (int)reservation.Status,
            reservation.Items.Select(i => new ReservationItemDto(i.Id, i.Name, i.Quantity, i.UnitPrice, i.TotalPrice)).ToList());
    }
}
