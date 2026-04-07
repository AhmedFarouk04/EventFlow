using MediatR;

namespace EventDrivenBookingPlatform.Modules.Pricing.Application.Queries.CalculatePrice;

public record CalculatePriceQuery(Guid ServiceId, DateTime StartDate, DateTime EndDate) : IRequest<PriceCalculationDto>;

public record PriceCalculationDto(decimal BasePrice, decimal TotalPrice, int Days);
