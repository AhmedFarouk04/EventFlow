using EventDrivenBookingPlatform.Modules.Reservations.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.ValueObjects;
using MediatR;

namespace EventDrivenBookingPlatform.Modules.Reservations.Application.Commands.CancelReservation;

public class CancelReservationHandler : IRequestHandler<CancelReservationCommand>
{
    private readonly IReservationRepository _reservationRepository;

    public CancelReservationHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(new ReservationId(request.ReservationId), cancellationToken);
        if (reservation is null)
        {
            throw new KeyNotFoundException($"Reservation {request.ReservationId} was not found.");
        }

        var result = reservation.Cancel(request.Reason);
        if (result.IsFailure)
        {
            throw new InvalidOperationException(result.Error);
        }

        await _reservationRepository.UpdateAsync(reservation, cancellationToken);
        await _reservationRepository.SaveChangesAsync(cancellationToken);
    }
}
