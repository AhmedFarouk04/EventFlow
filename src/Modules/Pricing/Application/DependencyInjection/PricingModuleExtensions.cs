using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;
using EventDrivenBookingPlatform.Modules.Pricing.Application.EventHandlers;
using EventDrivenBookingPlatform.Modules.Reservations.Contracts.IntegrationEvents;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace EventDrivenBookingPlatform.Modules.Pricing.Application.DependencyInjection;

public static class PricingModuleExtensions
{
    public static IServiceCollection AddPricingApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PricingModuleExtensions).Assembly));
        services.AddScoped<ReservationCreatedEventHandler>();
        return services;
    }

    public static void AddPricingSubscriptions(this IEventBus eventBus)
    {
        eventBus.Subscribe<ReservationCreatedIntegrationEvent, ReservationCreatedEventHandler>();
    }
}
