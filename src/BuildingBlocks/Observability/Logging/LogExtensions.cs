using Microsoft.Extensions.Logging;

namespace EventDrivenBookingPlatform.BuildingBlocks.Observability.Logging;

public static class LogExtensions
{
    public static IDisposable BeginCorrelationScope(this ILogger logger, string? correlationId)
    {
        return logger.BeginScope(new Dictionary<string, object?>
        {
            ["CorrelationId"] = correlationId
        }) ?? NullScope.Instance;
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();

        public void Dispose()
        {
        }
    }
}
