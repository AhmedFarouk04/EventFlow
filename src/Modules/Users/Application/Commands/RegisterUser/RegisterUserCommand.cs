using MediatR;

namespace EventDrivenBookingPlatform.Modules.Users.Application.Commands.RegisterUser;

public record RegisterUserCommand(string Email, string Password) : IRequest<Guid>;
