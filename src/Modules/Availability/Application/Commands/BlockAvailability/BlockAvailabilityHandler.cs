using EventDrivenBookingPlatform.Modules.Availability.Application.Interfaces;
using MediatR;

namespace EventDrivenBookingPlatform.Modules.Availability.Application.Commands.BlockAvailability;

public class BlockAvailabilityHandler : IRequestHandler<BlockAvailabilityCommand>
{
    private readonly IAvailabilityRepository _repository;

    public BlockAvailabilityHandler(IAvailabilityRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(BlockAvailabilityCommand request, CancellationToken cancellationToken)
    {
        var slot = await _repository.GetOrCreateAsync(request.ServiceId, request.Date, cancellationToken);

        var result = slot.Block(request.Quantity);
        if (result.IsFailure)
        {
            throw new InvalidOperationException(result.Error);
        }

        await _repository.SaveAsync(slot, cancellationToken);
    }
}
