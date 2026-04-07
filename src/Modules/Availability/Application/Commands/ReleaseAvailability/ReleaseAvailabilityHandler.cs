using EventDrivenBookingPlatform.Modules.Availability.Application.Interfaces;
using MediatR;

namespace EventDrivenBookingPlatform.Modules.Availability.Application.Commands.ReleaseAvailability;

public class ReleaseAvailabilityHandler : IRequestHandler<ReleaseAvailabilityCommand>
{
    private readonly IAvailabilityRepository _repository;

    public ReleaseAvailabilityHandler(IAvailabilityRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(ReleaseAvailabilityCommand request, CancellationToken cancellationToken)
    {
        var slot = await _repository.GetOrCreateAsync(request.ServiceId, request.Date, cancellationToken);

        var result = slot.Release(request.Quantity);
        if (result.IsFailure)
        {
            throw new InvalidOperationException(result.Error);
        }

        await _repository.SaveAsync(slot, cancellationToken);
    }
}
