using EventDrivenBookingPlatform.Modules.Availability.Application.Interfaces;
using MediatR;

namespace EventDrivenBookingPlatform.Modules.Availability.Application.Queries.CheckAvailability;

public class CheckAvailabilityHandler : IRequestHandler<CheckAvailabilityQuery, bool>
{
    private readonly IAvailabilityRepository _repository;

    public CheckAvailabilityHandler(IAvailabilityRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(CheckAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var slot = await _repository.FindAsync(request.ServiceId, request.Date, cancellationToken);
        if (slot is null)
        {
            return true;
        }

        return slot.Remaining >= request.Quantity;
    }
}
