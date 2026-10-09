using System.ComponentModel.DataAnnotations;

namespace Auth.Application.DTOs;

public sealed record LoginDto(
    [Required]
    [EmailAddress]
    string Email,

    [Required]
    string Password
);