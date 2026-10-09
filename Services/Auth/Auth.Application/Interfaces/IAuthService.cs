using Auth.Application.DTOs;

namespace Auth.Application.Interfaces;

public interface IAuthService
{
    Task<bool> RegisterAsync(
        RegisterDto dto,
        CancellationToken cancellationToken = default);
}