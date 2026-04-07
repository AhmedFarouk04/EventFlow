using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;
using EventDrivenBookingPlatform.Modules.Audit.Application.DependencyInjection;
using EventDrivenBookingPlatform.Modules.Availability.Application.DependencyInjection;
using EventDrivenBookingPlatform.Modules.Notifications.Application.DependencyInjection;
using EventDrivenBookingPlatform.Modules.Pricing.Application.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventDrivenBookingPlatform.Modules.Reservations.API.BackgroundServices;

public class EventBusSubscriptionsHostedService : IHostedService
{
    private readonly IEventBus _eventBus;

    public EventBusSubscriptionsHostedService(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _eventBus.AddAvailabilitySubscriptions();
        _eventBus.AddPricingSubscriptions();
        _eventBus.AddNotificationsSubscriptions();
        _eventBus.AddAuditSubscriptions();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
