using EventDrivenBookingPlatform.Modules.Reservations.Application.DTOs;
using MediatR;

namespace EventDrivenBookingPlatform.Modules.Reservations.Application.Queries.GetReservationsByCustomer;

public record GetReservationsByCustomerQuery(Guid CustomerId) : IRequest<IReadOnlyCollection<ReservationDto>>;
