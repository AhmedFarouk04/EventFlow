using MediatR;

namespace EventDrivenBookingPlatform.Modules.Reservations.Application.Commands.CancelReservation;

public record CancelReservationCommand(Guid ReservationId, string Reason) : IRequest;
