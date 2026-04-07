using EventDrivenBookingPlatform.Modules.Reservations.Application.DTOs;
using EventDrivenBookingPlatform.Modules.Reservations.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.ValueObjects;
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
        var reservation = await _repository.GetByIdAsync(new ReservationId(request.ReservationId), cancellationToken);

        if (reservation is null)
        {
            return null;
        }

        return ReservationDto.FromDomain(reservation);
    }
}
