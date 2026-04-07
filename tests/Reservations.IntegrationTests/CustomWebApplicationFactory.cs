using System.Reflection;
using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;
using EventDrivenBookingPlatform.BuildingBlocks.Messaging.Outbox;
using EventDrivenBookingPlatform.Modules.Audit.Application.DependencyInjection;
using EventDrivenBookingPlatform.Modules.Audit.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Audit.Infrastructure.Persistence;
using EventDrivenBookingPlatform.Modules.Availability.Application.DependencyInjection;
using EventDrivenBookingPlatform.Modules.Availability.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Availability.Infrastructure.Persistence;
using EventDrivenBookingPlatform.Modules.Notifications.Application.Abstractions;
using EventDrivenBookingPlatform.Modules.Notifications.Application.DependencyInjection;
using EventDrivenBookingPlatform.Modules.Notifications.Infrastructure.Persistence;
using EventDrivenBookingPlatform.Modules.Pricing.Application.DependencyInjection;
using EventDrivenBookingPlatform.Modules.Pricing.Infrastructure.Persistence;
using EventDrivenBookingPlatform.Modules.Users.Infrastructure.Persistence;
using EventDrivenBookingPlatform.Modules.Reservations.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Reservations.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"reservations-tests-{Guid.NewGuid()}";
    private bool _subscriptionsInitialized;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ReservationsDbContext>>();
            services.RemoveAll<ReservationsDbContext>();
            services.RemoveAll<DbContextOptions<AuditDbContext>>();
            services.RemoveAll<AuditDbContext>();
            services.RemoveAll<DbContextOptions<AvailabilityDbContext>>();
            services.RemoveAll<AvailabilityDbContext>();
            services.RemoveAll<DbContextOptions<PricingDbContext>>();
            services.RemoveAll<PricingDbContext>();
            services.RemoveAll<DbContextOptions<NotificationsDbContext>>();
            services.RemoveAll<NotificationsDbContext>();
            services.RemoveAll<DbContextOptions<UsersDbContext>>();
            services.RemoveAll<UsersDbContext>();
            services.RemoveAll<IEventBus>();
            services.RemoveAll<IEmailService>();

            services.AddSingleton<IEventBus, InProcessEventBus>();
            services.AddSingleton<FakeEmailService>();
            services.AddSingleton<IEmailService>(sp => sp.GetRequiredService<FakeEmailService>());

            services.AddDbContext<ReservationsDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));

            services.AddDbContext<AuditDbContext>(options =>
                options.UseInMemoryDatabase($"{_databaseName}-audit"));

            services.AddDbContext<AvailabilityDbContext>(options =>
                options.UseInMemoryDatabase($"{_databaseName}-availability"));

            services.AddDbContext<PricingDbContext>(options =>
                options.UseInMemoryDatabase($"{_databaseName}-pricing"));

            services.AddDbContext<NotificationsDbContext>(options =>
                options.UseInMemoryDatabase($"{_databaseName}-notifications"));

            services.AddDbContext<UsersDbContext>(options =>
                options.UseInMemoryDatabase($"{_databaseName}-users"));
        });
    }

    public void EnsureSubscriptions()
    {
        if (_subscriptionsInitialized)
        {
            return;
        }

        var eventBus = Services.GetRequiredService<IEventBus>();
        eventBus.AddAvailabilitySubscriptions();
        eventBus.AddPricingSubscriptions();
        eventBus.AddNotificationsSubscriptions();
        eventBus.AddAuditSubscriptions();
        _subscriptionsInitialized = true;
    }

    public FakeEmailService GetEmailService()
    {
        return Services.GetRequiredService<FakeEmailService>();
    }

    public IAuditLogStore GetAuditLogStore()
    {
        return Services.GetRequiredService<IAuditLogStore>();
    }

    public IAvailabilityRepository GetAvailabilityRepository()
    {
        return Services.GetRequiredService<IAvailabilityRepository>();
    }

    public async Task ProcessOutboxAsync()
    {
        var processor = new OutboxProcessor(
            Services,
            Options.Create(new OutboxProcessorOptions { BatchSize = 50, PollIntervalSeconds = 60 }),
            NullLogger<OutboxProcessor>.Instance);

        var method = typeof(OutboxProcessor).GetMethod("ProcessBatchAsync", BindingFlags.Instance | BindingFlags.NonPublic)!;
        await (Task)method.Invoke(processor, [CancellationToken.None])!;
    }

    public sealed class FakeEmailService : IEmailService
    {
        public List<EmailRecord> SentEmails { get; } = [];

        public Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
        {
            SentEmails.Add(new EmailRecord(to, subject, body));
            return Task.CompletedTask;
        }
    }

    public sealed record EmailRecord(string To, string Subject, string Body);

    private sealed class InProcessEventBus : IEventBus
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly Dictionary<string, List<Type>> _handlers = new();
        private readonly Dictionary<string, Type> _eventTypes = new();

        public InProcessEventBus(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IIntegrationEvent
        {
            var eventName = @event.GetType().Name;
            if (!_handlers.TryGetValue(eventName, out var handlers))
            {
                return;
            }

            using var scope = _scopeFactory.CreateScope();

            foreach (var handlerType in handlers)
            {
                var handler = scope.ServiceProvider.GetRequiredService(handlerType);
                var contractType = typeof(IIntegrationEventHandler<>).MakeGenericType(_eventTypes[eventName]);
                var method = contractType.GetMethod(nameof(IIntegrationEventHandler<IIntegrationEvent>.Handle))!;
                await (Task)method.Invoke(handler, [@event])!;
            }
        }

        public void Subscribe<T, TH>() where T : IIntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            var eventName = typeof(T).Name;

            _eventTypes[eventName] = typeof(T);

            if (!_handlers.TryGetValue(eventName, out var handlers))
            {
                handlers = [];
                _handlers[eventName] = handlers;
            }

            if (!handlers.Contains(typeof(TH)))
            {
                handlers.Add(typeof(TH));
            }
        }

        public void Unsubscribe<T, TH>() where T : IIntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            var eventName = typeof(T).Name;
            if (_handlers.TryGetValue(eventName, out var handlers))
            {
                handlers.Remove(typeof(TH));
            }
        }
    }
}
