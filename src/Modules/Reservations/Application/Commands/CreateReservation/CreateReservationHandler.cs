using EventDrivenBookingPlatform.Modules.Reservations.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.Aggregates;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.Rules;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.ValueObjects;
using MediatR;

namespace EventDrivenBookingPlatform.Modules.Reservations.Application.Commands.CreateReservation;

public class CreateReservationHandler : IRequestHandler<CreateReservationCommand, Guid>
{
    private readonly IReservationRepository _reservationRepository;

    public CreateReservationHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<Guid> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        var reservationId = ReservationId.New();
        var customerId = new CustomerId(request.CustomerId);
        var serviceId = new ServiceId(request.ServiceId);
        var dateRange = new DateRange(request.StartDate, request.EndDate);

        var hasOverlap = await _reservationRepository.HasOverlapAsync(customerId, dateRange, cancellationToken);
        var overlapRule = new ReservationCannotOverlapRule(hasOverlap, dateRange);
        if (overlapRule.IsBroken())
        {
            throw new InvalidOperationException(overlapRule.Message);
        }

        var createResult = Reservation.Create(reservationId, customerId, serviceId, dateRange);
        if (createResult.IsFailure)
        {
            throw new InvalidOperationException(createResult.Error);
        }

        var reservation = createResult.Value;

        if (request.Items is not null)
        {
            foreach (var item in request.Items)
            {
                var addItemResult = reservation.AddItem(item.Name, item.Quantity, item.UnitPrice);
                if (addItemResult.IsFailure)
                {
                    throw new InvalidOperationException(addItemResult.Error);
                }
            }
        }

        await _reservationRepository.AddAsync(reservation, cancellationToken);
        await _reservationRepository.SaveChangesAsync(cancellationToken);

        return reservation.Id;
    }
}
