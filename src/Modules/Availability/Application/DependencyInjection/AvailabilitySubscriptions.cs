using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;
using EventDrivenBookingPlatform.Modules.Availability.Application.EventHandlers;
using EventDrivenBookingPlatform.Modules.Reservations.Contracts.IntegrationEvents;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace EventDrivenBookingPlatform.Modules.Availability.Application.DependencyInjection;

public static class AvailabilitySubscriptions
{
    public static void AddAvailabilitySubscriptions(this IEventBus eventBus)
    {
        eventBus.Subscribe<ReservationCreatedIntegrationEvent, ReservationCreatedEventHandler>();
    }

    public static IServiceCollection AddAvailabilityApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AvailabilitySubscriptions).Assembly));
        services.AddScoped<ReservationCreatedEventHandler>();
        return services;
    }
}
