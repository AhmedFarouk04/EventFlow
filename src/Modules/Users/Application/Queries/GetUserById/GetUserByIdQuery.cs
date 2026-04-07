using MediatR;

namespace EventDrivenBookingPlatform.Modules.Users.Application.Queries.GetUserById;

public record GetUserByIdQuery(Guid UserId) : IRequest<UserDto?>;

public record UserDto(Guid Id, string Email);
