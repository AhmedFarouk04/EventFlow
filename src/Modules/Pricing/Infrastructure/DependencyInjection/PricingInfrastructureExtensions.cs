using EventDrivenBookingPlatform.Modules.Pricing.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Pricing.Domain;
using EventDrivenBookingPlatform.Modules.Pricing.Infrastructure.Persistence;
using EventDrivenBookingPlatform.Modules.Pricing.Infrastructure.Repositories;
using EventDrivenBookingPlatform.Modules.Pricing.Infrastructure.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventDrivenBookingPlatform.Modules.Pricing.Infrastructure.DependencyInjection;

public static class PricingInfrastructureExtensions
{
    public static IServiceCollection AddPricingInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PricingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Pricing")
                ?? "Server=(localdb)\\mssqllocaldb;Database=EventDrivenBookingPricingDb;Trusted_Connection=True;MultipleActiveResultSets=true"));

        services.AddScoped<IPricingRuleRepository, PricingRuleRepository>();
        services.AddScoped<IPriceCalculationStore, PriceCalculationStore>();
        services.AddScoped<PricingSeedService>();

        return services;
    }
}
