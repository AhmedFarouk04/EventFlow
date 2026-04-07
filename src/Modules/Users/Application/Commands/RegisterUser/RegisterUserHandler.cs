using EventDrivenBookingPlatform.Modules.Users.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Users.Domain;
using MediatR;

namespace EventDrivenBookingPlatform.Modules.Users.Application.Commands.RegisterUser;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _repository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserHandler(IUserRepository repository, IPasswordHasher passwordHasher)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException("User already exists.");
        }

        var user = User.Create(request.Email, _passwordHasher.Hash(request.Password));
        await _repository.AddAsync(user, cancellationToken);

        return user.Id;
    }
}
