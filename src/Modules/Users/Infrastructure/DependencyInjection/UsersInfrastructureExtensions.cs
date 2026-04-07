using EventDrivenBookingPlatform.Modules.Users.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Users.Infrastructure.Auth;
using EventDrivenBookingPlatform.Modules.Users.Infrastructure.Persistence;
using EventDrivenBookingPlatform.Modules.Users.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventDrivenBookingPlatform.Modules.Users.Infrastructure.DependencyInjection;

public static class UsersInfrastructureExtensions
{
    public static IServiceCollection AddUsersInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.AddDbContext<UsersDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Users")
                ?? "Server=(localdb)\\mssqllocaldb;Database=EventDrivenBookingUsersDb;Trusted_Connection=True;MultipleActiveResultSets=true"));
        services.AddScoped<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<IPasswordHasher, Sha256PasswordHasher>();
        services.AddSingleton<JwtTokenService>();

        return services;
    }
}
