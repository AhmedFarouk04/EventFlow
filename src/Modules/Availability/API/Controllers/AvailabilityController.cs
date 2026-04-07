using EventDrivenBookingPlatform.Modules.Availability.Application.Commands.BlockAvailability;
using EventDrivenBookingPlatform.Modules.Availability.Application.Commands.ReleaseAvailability;
using EventDrivenBookingPlatform.Modules.Availability.Application.Queries.CheckAvailability;
using EventDrivenBookingPlatform.Modules.Availability.Application.Queries.GetAvailableSlots;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventDrivenBookingPlatform.Modules.Availability.API.Controllers;

[ApiController]
[Route("api/availability")]
public class AvailabilityController : ControllerBase
{
    private readonly IMediator _mediator;

    public AvailabilityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("block")]
    public async Task<IActionResult> Block([FromBody] BlockAvailabilityCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("release")]
    public async Task<IActionResult> Release([FromBody] ReleaseAvailabilityCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpGet("check")]
    public async Task<IActionResult> Check([FromQuery] Guid serviceId, [FromQuery] DateTime date, [FromQuery] int quantity)
    {
        var available = await _mediator.Send(new CheckAvailabilityQuery(serviceId, date, quantity));
        return Ok(new { available });
    }

    [HttpGet("slots")]
    public async Task<IActionResult> Slots([FromQuery] Guid serviceId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
    {
        var result = await _mediator.Send(new GetAvailableSlotsQuery(serviceId, fromDate, toDate));
        return Ok(result);
    }
}
