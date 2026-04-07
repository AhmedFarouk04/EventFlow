using FluentValidation;

namespace EventDrivenBookingPlatform.Modules.Reservations.Application.Commands.CancelReservation;

public class CancelReservationValidator : AbstractValidator<CancelReservationCommand>
{
    public CancelReservationValidator()
    {
        RuleFor(x => x.ReservationId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(300);
    }
}
