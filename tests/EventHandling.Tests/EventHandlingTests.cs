using System.Reflection;
using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;
using EventDrivenBookingPlatform.BuildingBlocks.Messaging.Idempotency;
using EventDrivenBookingPlatform.BuildingBlocks.Messaging.Outbox;
using EventDrivenBookingPlatform.Modules.Reservations.Contracts.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace EventHandling.Tests;

public class EventHandlingTests
{
    [Fact]
    public async Task OutboxProcessor_ShouldPublishAndMarkMessageProcessed()
    {
        var message = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = typeof(ReservationCreatedIntegrationEvent).AssemblyQualifiedName!,
            Content = System.Text.Json.JsonSerializer.Serialize(new ReservationCreatedIntegrationEvent(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow)),
            OccurredOn = DateTime.UtcNow
        };

        var outboxStore = new FakeOutboxStore(message);
        var eventBus = new FakeEventBus();

        var services = new ServiceCollection();
        services.AddSingleton<IOutboxStore>(outboxStore);
        services.AddSingleton<IEventBus>(eventBus);
        var provider = services.BuildServiceProvider();

        var processor = new OutboxProcessor(
            provider,
            Options.Create(new OutboxProcessorOptions { BatchSize = 10, PollIntervalSeconds = 60 }),
            NullLogger<OutboxProcessor>.Instance);

        var method = typeof(OutboxProcessor).GetMethod("ProcessBatchAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;
        await (Task)method.Invoke(processor, [CancellationToken.None])!;

        Assert.Single(eventBus.PublishedEvents);
        Assert.Contains(message.Id, outboxStore.ProcessedMessages);
    }

    [Fact]
    public async Task IdempotentHandler_ShouldProcessEventOnlyOnce()
    {
        var store = new FakeProcessedMessageStore();
        var handler = new FakeReservationCreatedIdempotentHandler(store);
        var integrationEvent = new ReservationCreatedIntegrationEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow)
        {
            Id = Guid.NewGuid()
        };

        await handler.Handle(integrationEvent);
        await handler.Handle(integrationEvent);

        Assert.Equal(1, handler.ExecutionCount);
        Assert.Single(store.Messages);
    }

    private sealed class FakeEventBus : IEventBus
    {
        public List<IIntegrationEvent> PublishedEvents { get; } = [];

        public Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IIntegrationEvent
        {
            PublishedEvents.Add(@event);
            return Task.CompletedTask;
        }

        public void Subscribe<T, TH>() where T : IIntegrationEvent where TH : IIntegrationEventHandler<T>
        {
        }

        public void Unsubscribe<T, TH>() where T : IIntegrationEvent where TH : IIntegrationEventHandler<T>
        {
        }
    }

    private sealed class FakeOutboxStore : IOutboxStore
    {
        private readonly IReadOnlyList<OutboxMessage> _messages;

        public FakeOutboxStore(params OutboxMessage[] messages)
        {
            _messages = messages;
        }

        public List<Guid> ProcessedMessages { get; } = [];
        public List<Guid> FailedMessages { get; } = [];

        public Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<IReadOnlyList<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_messages);
        }

        public Task MarkAsFailedAsync(Guid id, string error, CancellationToken cancellationToken = default)
        {
            FailedMessages.Add(id);
            return Task.CompletedTask;
        }

        public Task MarkAsProcessedAsync(Guid id, CancellationToken cancellationToken = default)
        {
            ProcessedMessages.Add(id);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeProcessedMessageStore : IProcessedMessageStore
    {
        public List<ProcessedMessage> Messages { get; } = [];

        public Task<bool> ExistsAsync(Guid messageId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Messages.Any(x => x.MessageId == messageId));
        }

        public Task AddAsync(ProcessedMessage message, CancellationToken cancellationToken = default)
        {
            Messages.Add(message);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeReservationCreatedIdempotentHandler : IdempotentHandler<ReservationCreatedIntegrationEvent>
    {
        public FakeReservationCreatedIdempotentHandler(IProcessedMessageStore processedMessageStore)
            : base(processedMessageStore)
        {
        }

        public int ExecutionCount { get; private set; }

        protected override Task HandleCoreAsync(ReservationCreatedIntegrationEvent @event)
        {
            ExecutionCount++;
            return Task.CompletedTask;
        }
    }
}
