namespace Auth.Application.Interfaces;

public interface IEmailConfirmationTokenGenerator
{
    string GenerateToken();

    string HashToken(string token);
}