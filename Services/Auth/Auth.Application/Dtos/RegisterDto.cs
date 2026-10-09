using System.ComponentModel.DataAnnotations;

namespace Auth.Application.DTOs;

public sealed record RegisterDto(
    [Required]
    [MaxLength(100)]
    string FirstName,

    [Required]
    [MaxLength(100)]
    string Name,

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    string Email,

    [Required]
    [MinLength(8)]
    string Password


    //[RegularExpression(
    //@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
    //ErrorMessage = "Password must contain an uppercase letter, a lowercase letter and a number.")]
    //string Password
);