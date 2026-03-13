using EventDrivenBookingPlatform.Modules.Reservations.Application.DTOs;
using MediatR;

namespace EventDrivenBookingPlatform.Modules.Reservations.Application.Queries.GetReservationById;

public record GetReservationByIdQuery(Guid ReservationId) : IRequest<ReservationDto?>;