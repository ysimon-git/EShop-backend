using Auth.Application.DTOs;
using Auth.Application.Interfaces;
using Auth.Domain.Constants;
using Auth.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Auth.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IEmailConfirmationTokenGenerator _emailTokenGenerator;


    public AuthController(
       IUserRepository userRepository,
       IPasswordHasher passwordHasher,
       IJwtTokenGenerator jwtTokenGenerator,
       IEmailConfirmationTokenGenerator emailTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _emailTokenGenerator = emailTokenGenerator;
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register(
      RegisterDto dto,
      CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(
            dto.Email,
            cancellationToken);

        if (existingUser is not null)
            return BadRequest("Email already exists.");

        var passwordHash = _passwordHasher.Hash(dto.Password);

        var user = new User(
            dto.FirstName,
            dto.Name,
            dto.Email,
            passwordHash,
            Roles.User);

        // Generate email confirmation token
        var confirmationToken = _emailTokenGenerator.GenerateToken();

        // Only store the hash in the database
        var confirmationTokenHash = _emailTokenGenerator.HashToken(confirmationToken);


        user.SetEmailConfirmationToken(
            confirmationTokenHash,
            DateTime.UtcNow.AddHours(24));

        await _userRepository.AddAsync(
            user,
            cancellationToken);

        return Ok(new
        {
            Message = "User registered successfully.",
            ConfirmationToken = confirmationToken
        });
    }




    [HttpPost("login")]
    public async Task<IActionResult> Login(
    LoginDto dto,
    CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(
            dto.Email,
            cancellationToken);

        if (user is null)
            return Unauthorized("Invalid email or password.");

        if (!user.EmailConfirmed)
            return Unauthorized("Email has not been confirmed.");

        var passwordIsValid = _passwordHasher.Verify(
            dto.Password,
            user.PasswordHash);

        if (!passwordIsValid)
            return Unauthorized("Invalid email or password.");

        var token = _jwtTokenGenerator.GenerateToken(user);

        return Ok(new
        {
            Token = token
        });

    }


    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(
    ConfirmEmailDto dto,
    CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(
            dto.Email,
            cancellationToken);

        if (user is null)
            return BadRequest("Invalid confirmation request.");

        if (user.EmailConfirmed)
            return BadRequest("Email is already confirmed.");

        if (user.EmailConfirmationTokenHash is null ||
            user.EmailConfirmationTokenExpiresAtUtc is null)
        {
            return BadRequest("Invalid confirmation request.");
        }

        if (user.EmailConfirmationTokenExpiresAtUtc < DateTime.UtcNow)
            return BadRequest("Confirmation token has expired.");

        var tokenHash = _emailTokenGenerator.HashToken(dto.Token);

        if (tokenHash != user.EmailConfirmationTokenHash)
            return BadRequest("Invalid confirmation token.");

        user.ConfirmEmail();

        await _userRepository.UpdateAsync(
            user,
            cancellationToken);

        return Ok(new
        {
            Message = "Email confirmed successfully."
        });
    }


    //"token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJFU2hvcC5BcGkiLCJpc3MiOiJFU2hvcC5BdXRoIiwiZXhwIjoxNzkwNjEwNTE2LCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImQ5Zjk2YjQ2LTk4YTctNDQ1MC1iZWM1LWU3ODYzOGRiNjRiMyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6Inl2ZXNAdGVzdC5iZSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJpYXQiOjE3OTA2MDY5MTYsIm5iZiI6MTc5MDYwNjkxNn0.pWb_LbJJYFQvB-edght6AKp88AA2R4lZcvoiiZ51jsE"


    [Authorize]
    [HttpGet("test-auth")]
    public IActionResult TestAuth()
    {
        return Ok(new
        {
            Message = "You are authenticated.",
            UserId = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,

            Email = User.FindFirst(
                System.Security.Claims.ClaimTypes.Email)?.Value,

            Role = User.FindFirst(
                System.Security.Claims.ClaimTypes.Role)?.Value
        });
    }
}