namespace EventDrivenBookingPlatform.BuildingBlocks.EventBus.RabbitMQ;

public class RabbitMqOptions
{
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string VirtualHost { get; set; } = "/";
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string ExchangeName { get; set; } = "EventDrivenBookingExchange";
    public string ExchangeType { get; set; } = "direct";
    public string QueueName { get; set; } = "ReservationsQueue";
    public string DeadLetterExchangeName { get; set; } = "EventDrivenBookingExchange.dlx";
    public string DeadLetterQueueName { get; set; } = "ReservationsQueue.dlq";
    public int RetryCount { get; set; } = 5;
    public ushort PrefetchCount { get; set; } = 20;
}
