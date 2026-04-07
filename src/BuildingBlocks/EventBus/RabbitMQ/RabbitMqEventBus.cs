using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventDrivenBookingPlatform.BuildingBlocks.EventBus.RabbitMQ;

public class RabbitMqEventBus : IEventBus, IDisposable
{
    private readonly RabbitMqConnection _connection;
    private readonly ILogger<RabbitMqEventBus> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqOptions _options;
    private IModel? _consumerChannel;
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<Type, byte>> _handlers = new();
    private readonly ConcurrentDictionary<string, Type> _eventTypes = new();

    public RabbitMqEventBus(
        RabbitMqConnection connection,
        ILogger<RabbitMqEventBus> logger,
        IServiceScopeFactory scopeFactory,
        IOptions<RabbitMqOptions> options)
    {
        _connection = connection;
        _logger = logger;
        _scopeFactory = scopeFactory;
        _options = options.Value;
    }

    public Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IIntegrationEvent
    {
        if (!_connection.IsConnected)
        {
            _connection.TryConnect();
        }

        using var channel = _connection.GetConnection().CreateModel();
        DeclareMessagingTopology(channel);

        var eventName = @event.GetType().Name;
        var message = JsonSerializer.Serialize(@event, @event.GetType());
        var body = Encoding.UTF8.GetBytes(message);

        var properties = channel.CreateBasicProperties();
        properties.DeliveryMode = 2;
        properties.MessageId = @event.Id.ToString();
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        properties.Type = eventName;

        channel.BasicPublish(
            exchange: _options.ExchangeName,
            routingKey: eventName,
            mandatory: true,
            basicProperties: properties,
            body: body);

        _logger.LogInformation("Published integration event {EventName} with id {EventId}", eventName, @event.Id);

        return Task.CompletedTask;
    }

    public void Subscribe<T, TH>()
        where T : IIntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = typeof(T).Name;
        var handlerType = typeof(TH);

        _eventTypes.TryAdd(eventName, typeof(T));

        var handlers = _handlers.GetOrAdd(eventName, _ => new ConcurrentDictionary<Type, byte>());
        handlers.TryAdd(handlerType, 0);

        DoInternalSubscription(eventName);
    }

    public void Unsubscribe<T, TH>()
        where T : IIntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = typeof(T).Name;
        var handlerType = typeof(TH);

        if (_handlers.TryGetValue(eventName, out var handlers))
        {
            handlers.TryRemove(handlerType, out _);

            if (handlers.IsEmpty)
            {
                _handlers.TryRemove(eventName, out _);
                _eventTypes.TryRemove(eventName, out _);
            }
        }
    }

    private void DoInternalSubscription(string eventName)
    {
        if (!_connection.IsConnected)
        {
            _connection.TryConnect();
        }

        _consumerChannel ??= CreateConsumerChannel();

        _consumerChannel.QueueBind(
            queue: _options.QueueName,
            exchange: _options.ExchangeName,
            routingKey: eventName);
    }

    private IModel CreateConsumerChannel()
    {
        if (!_connection.IsConnected)
        {
            _connection.TryConnect();
        }

        var channel = _connection.GetConnection().CreateModel();
        DeclareMessagingTopology(channel);
        channel.BasicQos(0, _options.PrefetchCount, false);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (_, ea) =>
        {
            var eventName = ea.RoutingKey;
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());

            try
            {
                await ProcessEvent(eventName, message);
                channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message for event {EventName}. Sending to DLQ.", eventName);
                channel.BasicNack(ea.DeliveryTag, false, false);
            }
        };

        channel.BasicConsume(_options.QueueName, false, consumer);

        channel.CallbackException += (_, ea) =>
        {
            _logger.LogWarning(ea.Exception, "Consumer channel callback exception. Recreating channel.");
            _consumerChannel?.Dispose();
            _consumerChannel = CreateConsumerChannel();
        };

        return channel;
    }

    private void DeclareMessagingTopology(IModel channel)
    {
        channel.ExchangeDeclare(_options.ExchangeName, _options.ExchangeType, true, false, null);
        channel.ExchangeDeclare(_options.DeadLetterExchangeName, "direct", true, false, null);

        var queueArguments = new Dictionary<string, object>
        {
            ["x-dead-letter-exchange"] = _options.DeadLetterExchangeName,
            ["x-dead-letter-routing-key"] = _options.DeadLetterQueueName
        };

        channel.QueueDeclare(_options.QueueName, true, false, false, queueArguments);
        channel.QueueDeclare(_options.DeadLetterQueueName, true, false, false, null);
        channel.QueueBind(_options.DeadLetterQueueName, _options.DeadLetterExchangeName, _options.DeadLetterQueueName);
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        if (!_handlers.TryGetValue(eventName, out var subscriptions) || !_eventTypes.TryGetValue(eventName, out var eventType))
        {
            _logger.LogWarning("No handlers registered for event {EventName}", eventName);
            return;
        }

        var integrationEvent = JsonSerializer.Deserialize(message, eventType);
        if (integrationEvent is null)
        {
            throw new InvalidOperationException($"Unable to deserialize message for event {eventName}");
        }

        using var scope = _scopeFactory.CreateScope();

        foreach (var handlerType in subscriptions.Keys)
        {
            var handler = scope.ServiceProvider.GetService(handlerType);
            if (handler is null)
            {
                continue;
            }

            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
            var handleMethod = concreteType.GetMethod(nameof(IIntegrationEventHandler<IIntegrationEvent>.Handle))!;
            var task = (Task?)handleMethod.Invoke(handler, [integrationEvent]);
            if (task is not null)
            {
                await task;
            }
        }
    }

    public void Dispose()
    {
        _consumerChannel?.Dispose();
        _handlers.Clear();
        _eventTypes.Clear();
    }
}
