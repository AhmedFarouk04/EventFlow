using System.Text;
using System.Text.Json;
using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using global::RabbitMQ.Client;
using global::RabbitMQ.Client.Events;

namespace EventDrivenBookingPlatform.BuildingBlocks.EventBus.RabbitMQ;

public class RabbitMqEventBus : IEventBus, IDisposable
{
    private readonly RabbitMqConnection _connection;
    private readonly ILogger<RabbitMqEventBus> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqOptions _options;
    private IModel? _consumerChannel;
    private readonly Dictionary<string, List<Type>> _handlers;
    private readonly List<Type> _eventTypes;

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
        _handlers = new Dictionary<string, List<Type>>();
        _eventTypes = new List<Type>();
    }

    public void Publish(IIntegrationEvent @event)
    {
        if (!_connection.IsConnected) _connection.TryConnect();

        using var channel = _connection.GetConnection().CreateModel();
        var eventName = @event.GetType().Name;

        channel.ExchangeDeclare(exchange: _options.ExchangeName, type: "direct");

        var message = JsonSerializer.Serialize(@event, @event.GetType());
        var body = Encoding.UTF8.GetBytes(message);

        var properties = channel.CreateBasicProperties();
        properties.DeliveryMode = 2; // Persistent

        _logger.LogInformation("Publishing event to RabbitMQ: {EventId}", @event.Id);

        channel.BasicPublish(
            exchange: _options.ExchangeName,
            routingKey: eventName,
            mandatory: true,
            basicProperties: properties,
            body: body);
    }

    public void Subscribe<T, TH>()
        where T : IIntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = typeof(T).Name;
        var handlerType = typeof(TH);

        if (!_eventTypes.Contains(typeof(T)))
            _eventTypes.Add(typeof(T));

        if (!_handlers.ContainsKey(eventName))
            _handlers.Add(eventName, new List<Type>());

        if (_handlers[eventName].Any(h => h == handlerType))
            throw new ArgumentException($"Handler Type {handlerType.Name} already registered for '{eventName}'");

        _handlers[eventName].Add(handlerType);

        DoInternalSubscription(eventName);
    }

    public void Unsubscribe<T, TH>()
        where T : IIntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = typeof(T).Name;
        var handlerType = typeof(TH);

        if (_handlers.ContainsKey(eventName) && _handlers[eventName].Contains(handlerType))
        {
            _handlers[eventName].Remove(handlerType);
            if (!_handlers[eventName].Any())
            {
                _handlers.Remove(eventName);
                var eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);
                if (eventType != null) _eventTypes.Remove(eventType);
            }
        }
    }

    private void DoInternalSubscription(string eventName)
    {
        var containsKey = _handlers.ContainsKey(eventName);
        if (!containsKey) return;

        if (!_connection.IsConnected) _connection.TryConnect();

        if (_consumerChannel == null)
            _consumerChannel = CreateConsumerChannel();

        _consumerChannel.QueueBind(
            queue: _options.QueueName,
            exchange: _options.ExchangeName,
            routingKey: eventName);
    }

    private IModel CreateConsumerChannel()
    {
        if (!_connection.IsConnected) _connection.TryConnect();

        var channel = _connection.GetConnection().CreateModel();
        channel.ExchangeDeclare(exchange: _options.ExchangeName, type: "direct");
        channel.QueueDeclare(queue: _options.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var eventName = ea.RoutingKey;
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());

            try
            {
                await ProcessEvent(eventName, message);
                channel.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error Processing message \"{Message}\"", message);
                // Optionally Nack to requeue
            }
        };

        channel.BasicConsume(queue: _options.QueueName, autoAck: false, consumer: consumer);

        channel.CallbackException += (sender, ea) =>
        {
            _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");
            _consumerChannel?.Dispose();
            _consumerChannel = CreateConsumerChannel();
        };

        return channel;
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        if (_handlers.ContainsKey(eventName))
        {
            using var scope = _scopeFactory.CreateScope();
            var subscriptions = _handlers[eventName];
            foreach (var subscription in subscriptions)
            {
                var handler = scope.ServiceProvider.GetService(subscription);
                if (handler == null) continue;

                var eventType = _eventTypes.SingleOrDefault(t => t.Name == eventName);
                if (eventType == null) continue;

                var integrationEvent = JsonSerializer.Deserialize(message, eventType);
                var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                await (Task)concreteType.GetMethod("Handle")!.Invoke(handler, new[] { integrationEvent })!;
            }
        }
    }

    public void Dispose()
    {
        _consumerChannel?.Dispose();
        _handlers.Clear();
    }
}