using EventDrivenBookingPlatform.Modules.Availability.Application.Interfaces;
using MediatR;

namespace EventDrivenBookingPlatform.Modules.Availability.Application.Queries.GetAvailableSlots;

public class GetAvailableSlotsHandler : IRequestHandler<GetAvailableSlotsQuery, IReadOnlyCollection<AvailableSlotDto>>
{
    private readonly IAvailabilityRepository _repository;

    public GetAvailableSlotsHandler(IAvailabilityRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<AvailableSlotDto>> Handle(GetAvailableSlotsQuery request, CancellationToken cancellationToken)
    {
        var slots = await _repository.GetByServiceRangeAsync(request.ServiceId, request.FromDate, request.ToDate, cancellationToken);
        return slots.Select(s => new AvailableSlotDto(s.Date, s.Remaining)).ToList();
    }
}
