using EventDrivenBookingPlatform.Modules.Reservations.Domain.Aggregates;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.ValueObjects;
using EventDrivenBookingPlatform.Modules.Reservations.Infrastructure.Persistence;
using EventDrivenBookingPlatform.Modules.Reservations.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Reservations.IntegrationTests;

public class ReservationRepositoryTests
{
    [Fact]
    public async Task GetByIdAsync_ShouldReturnPersistedReservation()
    {
        var options = new DbContextOptionsBuilder<ReservationsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var dbContext = new ReservationsDbContext(options);
        var repository = new ReservationRepository(dbContext);

        var reservation = Reservation.Create(
            ReservationId.New(),
            new CustomerId(Guid.NewGuid()),
            new ServiceId(Guid.NewGuid()),
            new DateRange(DateTime.UtcNow.Date.AddDays(3), DateTime.UtcNow.Date.AddDays(5))).Value;

        await repository.AddAsync(reservation);
        await repository.SaveChangesAsync();

        var loaded = await repository.GetByIdAsync(new ReservationId(reservation.Id));

        Assert.NotNull(loaded);
        Assert.Equal(reservation.Id, loaded!.Id);
    }
}
