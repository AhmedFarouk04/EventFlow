using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;
using EventDrivenBookingPlatform.Modules.Notifications.Application.EventHandlers;
using EventDrivenBookingPlatform.Modules.Reservations.Contracts.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;

namespace EventDrivenBookingPlatform.Modules.Notifications.Application.DependencyInjection;

public static class NotificationsModuleExtensions
{
    public static IServiceCollection AddNotificationsApplication(this IServiceCollection services)
    {
        services.AddScoped<ReservationCreatedEventHandler>();
        services.AddScoped<ReservationConfirmedEventHandler>();
        return services;
    }

    public static void AddNotificationsSubscriptions(this IEventBus eventBus)
    {
        eventBus.Subscribe<ReservationCreatedIntegrationEvent, ReservationCreatedEventHandler>();
        eventBus.Subscribe<ReservationConfirmedIntegrationEvent, ReservationConfirmedEventHandler>();
    }
}
