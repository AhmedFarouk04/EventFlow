using EventDrivenBookingPlatform.Modules.Reservations.Application.Behaviors;
using EventDrivenBookingPlatform.Modules.Reservations.Application.Commands.CreateReservation;
using EventDrivenBookingPlatform.Modules.Reservations.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Reservations.Infrastructure.Persistence;
using EventDrivenBookingPlatform.Modules.Reservations.Infrastructure.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventDrivenBookingPlatform.Modules.Reservations.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReservationsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ReservationsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Reservations")
                ?? "Server=(localdb)\\mssqllocaldb;Database=EventDrivenBookingDB;Trusted_Connection=True;MultipleActiveResultSets=true"));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<CreateReservationCommand>();
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        services.AddValidatorsFromAssemblyContaining<CreateReservationValidator>();

        services.AddScoped<IReservationRepository, ReservationRepository>();

        return services;
    }
}
