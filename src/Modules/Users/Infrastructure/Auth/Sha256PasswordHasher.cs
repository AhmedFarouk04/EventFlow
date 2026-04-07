using System.Security.Cryptography;
using System.Text;
using EventDrivenBookingPlatform.Modules.Users.Application.Interfaces;

namespace EventDrivenBookingPlatform.Modules.Users.Infrastructure.Auth;

public class Sha256PasswordHasher : IPasswordHasher
{
    public string Hash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }
}
