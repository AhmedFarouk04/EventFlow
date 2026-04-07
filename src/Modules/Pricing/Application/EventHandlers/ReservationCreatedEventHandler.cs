using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;
using EventDrivenBookingPlatform.Modules.Pricing.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Pricing.Application.Queries.CalculatePrice;
using EventDrivenBookingPlatform.Modules.Pricing.Contracts.IntegrationEvents;
using EventDrivenBookingPlatform.Modules.Reservations.Contracts.IntegrationEvents;
using MediatR;

namespace EventDrivenBookingPlatform.Modules.Pricing.Application.EventHandlers;

public class ReservationCreatedEventHandler : IIntegrationEventHandler<ReservationCreatedIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly IEventBus _eventBus;
    private readonly IPriceCalculationStore _priceCalculationStore;

    public ReservationCreatedEventHandler(IMediator mediator, IEventBus eventBus, IPriceCalculationStore priceCalculationStore)
    {
        _mediator = mediator;
        _eventBus = eventBus;
        _priceCalculationStore = priceCalculationStore;
    }

    public async Task Handle(ReservationCreatedIntegrationEvent @event)
    {
        var price = await _mediator.Send(new CalculatePriceQuery(@event.ServiceId, @event.StartDate, @event.EndDate));
        await _priceCalculationStore.AddAsync(@event.ReservationId, @event.ServiceId, price.BasePrice, price.TotalPrice, price.Days);

        await _eventBus.PublishAsync(new PriceCalculatedIntegrationEvent(
            @event.ReservationId,
            @event.ServiceId,
            price.Days,
            price.TotalPrice,
            DateTime.UtcNow));
    }
}
