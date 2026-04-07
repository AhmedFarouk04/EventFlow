using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace EventDrivenBookingPlatform.Modules.Users.Application.DependencyInjection;

public static class UsersApplicationExtensions
{
    public static IServiceCollection AddUsersApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UsersApplicationExtensions).Assembly));
        return services;
    }
}
