using EventDrivenBookingPlatform.Modules.Reservations.Application.DTOs;
using EventDrivenBookingPlatform.Modules.Reservations.Application.Interfaces;
using MediatR;

namespace EventDrivenBookingPlatform.Modules.Reservations.Application.Queries.GetReservationById;

public class GetReservationByIdHandler : IRequestHandler<GetReservationByIdQuery, ReservationDto?>
{
    private readonly IReservationRepository _repository;

    public GetReservationByIdHandler(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<ReservationDto?> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
    {
        var reservation = await _repository.GetByIdAsync(request.ReservationId);

        if (reservation == null) return null;

        return new ReservationDto(
            reservation.Id,
            reservation.CustomerInfo.FullName,
            reservation.CustomerInfo.Email,
            reservation.TripDetails.TripName,
            reservation.TripDetails.Destination,
            reservation.PriceDetails.TotalPrice,
            reservation.PriceDetails.Currency,
            reservation.CheckInDate,
            reservation.CheckOutDate,
            (int)reservation.Status
        );
    }
}