namespace EventDrivenBookingPlatform.BuildingBlocks.SharedKernel;

public static class GuardClauses
{
    public static void NullOrEmpty(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{parameterName} cannot be null or empty.");
    }

    public static void NegativeOrZero(decimal value, string parameterName)
    {
        if (value <= 0)
            throw new ArgumentException($"{parameterName} must be greater than zero.");
    }

    public static void NotNull(object value, string parameterName)
    {
        if (value is null)
            throw new ArgumentNullException(parameterName);
    }
}