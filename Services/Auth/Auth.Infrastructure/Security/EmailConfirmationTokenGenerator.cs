using Auth.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Auth.Infrastructure.Security;

public sealed class EmailConfirmationTokenGenerator
    : IEmailConfirmationTokenGenerator
{
    public string GenerateToken()
    {
        return Convert.ToHexString(
            RandomNumberGenerator.GetBytes(32));
    }

    public string HashToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);

        var hash = SHA256.HashData(bytes);

        return Convert.ToHexString(hash);
    }
}