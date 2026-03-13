using EventDrivenBookingPlatform.Modules.Reservations.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Reservations.Infrastructure.Persistence;
using EventDrivenBookingPlatform.Modules.Reservations.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using EventDrivenBookingPlatform.Modules.Reservations.Application.Commands.CreateReservation;

// --- Messaging, Event Bus & Outbox Usings ---
using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;
using EventDrivenBookingPlatform.BuildingBlocks.EventBus.RabbitMQ;
using EventDrivenBookingPlatform.BuildingBlocks.Messaging.Outbox;
using EventDrivenBookingPlatform.Modules.Reservations.Infrastructure.Messaging.Outbox;
using Microsoft.Extensions.Options;
using global::RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. Add Infrastructure (Database)
// Note: In a real project, the connection string comes from appsettings.json.
// We are using a local SQL Server for development.
builder.Services.AddDbContext<ReservationsDbContext>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=EventDrivenBookingDB;Trusted_Connection=True;MultipleActiveResultSets=true"));

// 3. Add Repositories
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

// 4. Add MediatR (Application Layer)
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssemblyContaining<CreateReservationCommand>();
});

// 5. --- Messaging & Event Bus Configuration ---

// Setup RabbitMQ Options from appsettings.json
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));

// Setup RabbitMQ Connection
builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    var options = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
    return new ConnectionFactory
    {
        HostName = options.HostName,
        UserName = options.UserName,
        Password = options.Password,
        DispatchConsumersAsync = true // Important for async Handlers
    };
});
builder.Services.AddSingleton<RabbitMqConnection>();

// Setup Event Bus
builder.Services.AddSingleton<IEventBus, RabbitMqEventBus>();

// Setup Outbox Store
builder.Services.AddScoped<IOutboxStore, OutboxStore>();

// Setup Background Services (The Worker that reads from Outbox and sends to RabbitMQ)
builder.Services.AddHostedService<OutboxProcessor>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Note: We will add Middlewares (like Error Handling & Correlation ID) here later

app.Run();