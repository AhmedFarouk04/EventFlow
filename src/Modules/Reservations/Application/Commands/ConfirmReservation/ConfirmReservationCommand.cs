using MediatR;

namespace EventDrivenBookingPlatform.Modules.Reservations.Application.Commands.ConfirmReservation;

public record ConfirmReservationCommand(Guid ReservationId) : IRequest;
