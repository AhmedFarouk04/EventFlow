using System.Collections.Concurrent;

namespace EventDrivenBookingPlatform.BuildingBlocks.Observability.Correlation;

public static class CorrelationContext
{
    private static readonly AsyncLocal<string?> CorrelationIdStore = new();

    public static string? CorrelationId
    {
        get => CorrelationIdStore.Value;
        set => CorrelationIdStore.Value = value;
    }

    public static IDisposable Set(string correlationId)
    {
        CorrelationId = correlationId;
        return new CorrelationCleanup();
    }

    private sealed class CorrelationCleanup : IDisposable
    {
        public void Dispose()
        {
            CorrelationId = null;
        }
    }
}
