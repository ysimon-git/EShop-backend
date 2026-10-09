namespace Auth.Domain.Entities;

public sealed class User
{
    public Guid Id { get; private set; }

    public string FirstName { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }

    public string PasswordHash { get; private set; }

    public string Role { get; private set; }

    public bool EmailConfirmed { get; private set; }

    public string? EmailConfirmationTokenHash { get; private set; }

    public DateTime? EmailConfirmationTokenExpiresAtUtc { get; private set; }


    private User()
    {
    }

    public User(
        string firstName,
        string name,
        string email,
        string passwordHash,
        string role)
    {
        Id = Guid.NewGuid();

        FirstName = firstName;
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        EmailConfirmed = false;
    }


    public void SetEmailConfirmationToken(
    string tokenHash,
    DateTime expiresAtUtc)
    {
        EmailConfirmationTokenHash = tokenHash;
        EmailConfirmationTokenExpiresAtUtc = expiresAtUtc;
    }

    public void ConfirmEmail()
    {
        EmailConfirmed = true;
        EmailConfirmationTokenHash = null;
        EmailConfirmationTokenExpiresAtUtc = null;
    }
}