using EventDrivenBookingPlatform.Modules.Users.Application.Interfaces;
using MediatR;

namespace EventDrivenBookingPlatform.Modules.Users.Application.Queries.GetUserById;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserRepository _repository;

    public GetUserByIdHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.UserId, cancellationToken);
        return user is null ? null : new UserDto(user.Id, user.Email);
    }
}
