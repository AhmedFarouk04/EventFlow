using EventDrivenBookingPlatform.Modules.Availability.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Availability.Infrastructure.Persistence;
using EventDrivenBookingPlatform.Modules.Availability.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventDrivenBookingPlatform.Modules.Availability.Infrastructure.DependencyInjection;

public static class AvailabilityInfrastructureExtensions
{
    public static IServiceCollection AddAvailabilityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AvailabilityDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Availability")
                ?? "Server=(localdb)\\mssqllocaldb;Database=EventDrivenBookingAvailabilityDb;Trusted_Connection=True;MultipleActiveResultSets=true"));

        services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();
        return services;
    }
}
