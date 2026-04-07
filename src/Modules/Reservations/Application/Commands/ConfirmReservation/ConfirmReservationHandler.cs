using EventDrivenBookingPlatform.Modules.Reservations.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.ValueObjects;
using MediatR;

namespace EventDrivenBookingPlatform.Modules.Reservations.Application.Commands.ConfirmReservation;

public class ConfirmReservationHandler : IRequestHandler<ConfirmReservationCommand>
{
    private readonly IReservationRepository _reservationRepository;

    public ConfirmReservationHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task Handle(ConfirmReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(new ReservationId(request.ReservationId), cancellationToken);
        if (reservation is null)
        {
            throw new KeyNotFoundException($"Reservation {request.ReservationId} was not found.");
        }

        var result = reservation.Confirm();
        if (result.IsFailure)
        {
            throw new InvalidOperationException(result.Error);
        }

        await _reservationRepository.UpdateAsync(reservation, cancellationToken);
        await _reservationRepository.SaveChangesAsync(cancellationToken);
    }
}
