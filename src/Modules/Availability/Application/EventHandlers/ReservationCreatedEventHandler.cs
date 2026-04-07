using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;
using EventDrivenBookingPlatform.Modules.Availability.Application.Commands.BlockAvailability;
using EventDrivenBookingPlatform.Modules.Availability.Contracts.IntegrationEvents;
using EventDrivenBookingPlatform.Modules.Reservations.Contracts.IntegrationEvents;
using MediatR;

namespace EventDrivenBookingPlatform.Modules.Availability.Application.EventHandlers;

public class ReservationCreatedEventHandler : IIntegrationEventHandler<ReservationCreatedIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly IEventBus _eventBus;

    public ReservationCreatedEventHandler(IMediator mediator, IEventBus eventBus)
    {
        _mediator = mediator;
        _eventBus = eventBus;
    }

    public async Task Handle(ReservationCreatedIntegrationEvent @event)
    {
        var reservationDates = Enumerable
            .Range(0, Math.Max(1, (@event.EndDate.Date - @event.StartDate.Date).Days))
            .Select(offset => @event.StartDate.Date.AddDays(offset));

        foreach (var reservationDate in reservationDates)
        {
            await _mediator.Send(new BlockAvailabilityCommand(@event.ServiceId, reservationDate, 1));

            await _eventBus.PublishAsync(new AvailabilityBlockedIntegrationEvent(
                Guid.NewGuid(),
                @event.ReservationId,
                @event.ServiceId,
                reservationDate,
                1,
                DateTime.UtcNow));
        }
    }
}
