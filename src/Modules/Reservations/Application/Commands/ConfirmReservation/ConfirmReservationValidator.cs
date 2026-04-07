using FluentValidation;

namespace EventDrivenBookingPlatform.Modules.Reservations.Application.Commands.ConfirmReservation;

public class ConfirmReservationValidator : AbstractValidator<ConfirmReservationCommand>
{
    public ConfirmReservationValidator()
    {
        RuleFor(x => x.ReservationId).NotEmpty();
    }
}
