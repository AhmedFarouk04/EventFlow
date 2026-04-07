using EventDrivenBookingPlatform.Modules.Notifications.Application.Abstractions;
using EventDrivenBookingPlatform.Modules.Notifications.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventDrivenBookingPlatform.Modules.Notifications.Infrastructure;

public static class NotificationsInfrastructureExtensions
{
    public static IServiceCollection AddNotificationsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NotificationsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Notifications")
                ?? "Server=(localdb)\\mssqllocaldb;Database=EventDrivenBookingNotificationsDb;Trusted_Connection=True;MultipleActiveResultSets=true"));

        services.AddScoped<IEmailService, EmailService>();
        return services;
    }
}
