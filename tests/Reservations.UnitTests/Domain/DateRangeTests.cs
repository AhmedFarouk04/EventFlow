using EventDrivenBookingPlatform.Modules.Reservations.Domain.ValueObjects;

namespace Reservations.UnitTests.Domain;

public class DateRangeTests
{
    [Fact]
    public void Ctor_ShouldThrow_WhenStartDateIsNotBeforeEndDate()
    {
        var now = DateTime.UtcNow;

        Assert.Throws<ArgumentException>(() => new DateRange(now, now));
    }

    [Fact]
    public void Ctor_ShouldCreate_WhenRangeIsValid()
    {
        var start = DateTime.UtcNow.AddDays(1);
        var end = DateTime.UtcNow.AddDays(2);

        var range = new DateRange(start, end);

        Assert.Equal(start, range.StartDate);
        Assert.Equal(end, range.EndDate);
    }
}
