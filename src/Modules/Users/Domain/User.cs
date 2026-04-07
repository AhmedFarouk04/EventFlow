namespace EventDrivenBookingPlatform.Modules.Users.Domain;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

    private User() { }

    private User(Guid id, string email, string passwordHash)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
    }

    public static User Create(string email, string passwordHash)
    {
        return new User(Guid.NewGuid(), email, passwordHash);
    }
}
