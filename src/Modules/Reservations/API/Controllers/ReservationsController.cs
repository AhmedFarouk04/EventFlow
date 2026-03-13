using EventDrivenBookingPlatform.Modules.Reservations.Application.Commands.CreateReservation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventDrivenBookingPlatform.Modules.Reservations.API.Controllers;

[ApiController]
[Route("api/modules/reservations")]
public class ReservationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReservationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { Error = result.Error });
        }

        return CreatedAtAction(nameof(GetReservation), new { id = result.Value }, result.Value);
    }

    [HttpGet("{id}")]
    public IActionResult GetReservation(Guid id)
    {
        // Placeholder for the Query side (CQRS)
        // We will implement this later when we fill the Queries folder
        return Ok(new { Message = $"Reservation {id} details will be fetched here." });
    }
}