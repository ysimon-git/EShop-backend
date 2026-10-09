using System.ComponentModel.DataAnnotations;

namespace Auth.Application.DTOs;

public sealed record ConfirmEmailDto(
    [Required]
    [EmailAddress]
    string Email,

    [Required]
    string Token
);