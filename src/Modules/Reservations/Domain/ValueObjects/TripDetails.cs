using EventDrivenBookingPlatform.BuildingBlocks.SharedKernel;

namespace EventDrivenBookingPlatform.Modules.Reservations.Domain.ValueObjects;

public class TripDetails : ValueObject
{
    public Guid TripId { get; }
    public string TripName { get; }
    public string Destination { get; }
    public int Duration { get; }

    private TripDetails(Guid tripId, string tripName, string destination, int duration)
    {
        TripId = tripId;
        TripName = tripName;
        Destination = destination;
        Duration = duration;
    }

    public static Result<TripDetails> Create(Guid tripId, string tripName, string destination, int duration)
    {
        if (tripId == Guid.Empty) return Result<TripDetails>.Failure("TripId cannot be empty.");
        if (string.IsNullOrWhiteSpace(tripName)) return Result<TripDetails>.Failure("TripName is required.");
        if (string.IsNullOrWhiteSpace(destination)) return Result<TripDetails>.Failure("Destination is required.");
        if (duration < 1 || duration > 30) return Result<TripDetails>.Failure("Duration must be between 1 and 30 days.");

        return Result<TripDetails>.Success(new TripDetails(tripId, tripName, destination, duration));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TripId;
        yield return TripName;
        yield return Destination;
        yield return Duration;
    }
}