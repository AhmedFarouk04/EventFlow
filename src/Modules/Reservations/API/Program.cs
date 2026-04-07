using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;
using EventDrivenBookingPlatform.BuildingBlocks.EventBus.RabbitMQ;
using EventDrivenBookingPlatform.BuildingBlocks.Messaging.Outbox;
using EventDrivenBookingPlatform.BuildingBlocks.Observability.Correlation;
using EventDrivenBookingPlatform.Modules.Audit.Application.DependencyInjection;
using EventDrivenBookingPlatform.Modules.Audit.Infrastructure.DependencyInjection;
using EventDrivenBookingPlatform.Modules.Availability.Application.DependencyInjection;
using EventDrivenBookingPlatform.Modules.Availability.Infrastructure.DependencyInjection;
using EventDrivenBookingPlatform.Modules.Notifications.Application.DependencyInjection;
using EventDrivenBookingPlatform.Modules.Notifications.Infrastructure;
using EventDrivenBookingPlatform.Modules.Pricing.Application.DependencyInjection;
using EventDrivenBookingPlatform.Modules.Pricing.Infrastructure.DependencyInjection;
using EventDrivenBookingPlatform.Modules.Reservations.API.BackgroundServices;
using EventDrivenBookingPlatform.Modules.Reservations.API.Extensions;
using EventDrivenBookingPlatform.Modules.Reservations.API.Middlewares;
using EventDrivenBookingPlatform.Modules.Reservations.Infrastructure.Messaging.Outbox;
using EventDrivenBookingPlatform.Modules.Users.Application.DependencyInjection;
using EventDrivenBookingPlatform.Modules.Users.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "Reservations.API"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddReservationsModule(builder.Configuration);
builder.Services.AddAvailabilityApplication();
builder.Services.AddAvailabilityInfrastructure(builder.Configuration);
builder.Services.AddPricingApplication();
builder.Services.AddPricingInfrastructure(builder.Configuration);
builder.Services.AddNotificationsApplication();
builder.Services.AddNotificationsInfrastructure(builder.Configuration);
builder.Services.AddUsersApplication();
builder.Services.AddUsersInfrastructure(builder.Configuration);
builder.Services.AddAuditApplication();
builder.Services.AddAuditInfrastructure(builder.Configuration);

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.Configure<OutboxProcessorOptions>(builder.Configuration.GetSection("Outbox"));

builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    var options = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
    return new ConnectionFactory
    {
        HostName = options.HostName,
        Port = options.Port,
        VirtualHost = options.VirtualHost,
        UserName = options.UserName,
        Password = options.Password,
        DispatchConsumersAsync = true
    };
});

builder.Services.AddSingleton<RabbitMqConnection>();
builder.Services.AddSingleton<IEventBus, RabbitMqEventBus>();
builder.Services.AddScoped<IOutboxStore, OutboxStore>();
builder.Services.AddHostedService<OutboxProcessor>();
builder.Services.AddHostedService<EventBusSubscriptionsHostedService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var pricingSeedService = scope.ServiceProvider.GetRequiredService<PricingSeedService>();
    await pricingSeedService.SeedDefaultRuleAsync();
}

app.UseSerilogRequestLogging();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;
