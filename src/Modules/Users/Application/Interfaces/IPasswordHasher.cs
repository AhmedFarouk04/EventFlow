namespace EventDrivenBookingPlatform.Modules.Users.Application.Interfaces;

public interface IPasswordHasher
{
    string Hash(string input);
}
