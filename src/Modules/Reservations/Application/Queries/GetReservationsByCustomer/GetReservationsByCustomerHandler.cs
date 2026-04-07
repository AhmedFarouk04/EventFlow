using EventDrivenBookingPlatform.Modules.Reservations.Application.DTOs;
using EventDrivenBookingPlatform.Modules.Reservations.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.ValueObjects;
using MediatR;

namespace EventDrivenBookingPlatform.Modules.Reservations.Application.Queries.GetReservationsByCustomer;

public class GetReservationsByCustomerHandler : IRequestHandler<GetReservationsByCustomerQuery, IReadOnlyCollection<ReservationDto>>
{
    private readonly IReservationRepository _repository;

    public GetReservationsByCustomerHandler(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<ReservationDto>> Handle(GetReservationsByCustomerQuery request, CancellationToken cancellationToken)
    {
        var reservations = await _repository.GetByCustomerIdAsync(new CustomerId(request.CustomerId), cancellationToken);
        return reservations.Select(ReservationDto.FromDomain).ToList();
    }
}
