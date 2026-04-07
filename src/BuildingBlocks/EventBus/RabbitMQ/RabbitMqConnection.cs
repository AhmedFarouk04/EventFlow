using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace EventDrivenBookingPlatform.BuildingBlocks.EventBus.RabbitMQ;

public class RabbitMqConnection : IDisposable
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<RabbitMqConnection> _logger;
    private readonly RabbitMqOptions _options;
    private IConnection? _connection;
    private bool _disposed;
    private readonly object _syncRoot = new();

    public RabbitMqConnection(
        IConnectionFactory connectionFactory,
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqConnection> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
        _options = options.Value;
    }

    public bool IsConnected => _connection is { IsOpen: true } && !_disposed;

    public IConnection GetConnection()
    {
        if (!IsConnected && !TryConnect())
        {
            throw new InvalidOperationException("Unable to establish RabbitMQ connection.");
        }

        return _connection!;
    }

    public bool TryConnect()
    {
        if (_disposed)
        {
            return false;
        }

        lock (_syncRoot)
        {
            for (var attempt = 1; attempt <= _options.RetryCount; attempt++)
            {
                try
                {
                    _logger.LogInformation("Trying RabbitMQ connection attempt {Attempt}/{RetryCount}", attempt, _options.RetryCount);
                    _connection = _connectionFactory.CreateConnection();

                    if (!IsConnected)
                    {
                        continue;
                    }

                    _connection.ConnectionShutdown += OnConnectionShutdown;
                    _connection.CallbackException += OnCallbackException;
                    _connection.ConnectionBlocked += OnConnectionBlocked;

                    _logger.LogInformation("RabbitMQ connected to {Host}:{Port}", _options.HostName, _options.Port);
                    return true;
                }
                catch (BrokerUnreachableException ex)
                {
                    _logger.LogWarning(ex, "RabbitMQ unreachable on attempt {Attempt}", attempt);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Unexpected RabbitMQ connection error on attempt {Attempt}", attempt);
                }

                Thread.Sleep(TimeSpan.FromSeconds(Math.Min(2 * attempt, 10)));
            }

            _logger.LogCritical("RabbitMQ connection failed after {RetryCount} retries", _options.RetryCount);
            return false;
        }
    }

    private void OnConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
    {
        if (_disposed)
        {
            return;
        }

        _logger.LogWarning("RabbitMQ connection blocked. Reconnecting.");
        TryConnect();
    }

    private void OnCallbackException(object? sender, CallbackExceptionEventArgs e)
    {
        if (_disposed)
        {
            return;
        }

        _logger.LogWarning(e.Exception, "RabbitMQ callback exception. Reconnecting.");
        TryConnect();
    }

    private void OnConnectionShutdown(object? sender, ShutdownEventArgs reason)
    {
        if (_disposed)
        {
            return;
        }

        _logger.LogWarning("RabbitMQ connection shutdown ({ReplyCode}:{ReplyText}). Reconnecting.", reason.ReplyCode, reason.ReplyText);
        TryConnect();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        try
        {
            _connection?.Dispose();
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "Error disposing RabbitMQ connection");
        }
    }
}
