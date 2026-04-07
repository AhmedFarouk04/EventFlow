using EventDrivenBookingPlatform.Modules.Audit.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Audit.Infrastructure.Persistence;
using EventDrivenBookingPlatform.Modules.Audit.Infrastructure.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventDrivenBookingPlatform.Modules.Audit.Infrastructure.DependencyInjection;

public static class AuditInfrastructureExtensions
{
    public static IServiceCollection AddAuditInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuditDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Audit")
                ?? "Server=(localdb)\\mssqllocaldb;Database=EventDrivenBookingAuditDb;Trusted_Connection=True;MultipleActiveResultSets=true"));

        services.AddScoped<IAuditLogStore, AuditLogStore>();
        return services;
    }
}
