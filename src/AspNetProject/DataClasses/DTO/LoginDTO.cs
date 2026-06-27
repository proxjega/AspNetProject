using System.ComponentModel.DataAnnotations;

namespace AspNetProject.DTOs;

public record LoginDTO
{
    [Required]
    [EmailAddress]
    public required string Email { get; init; }
    [Required]
    [MinLength(6)]
    public required string Password { get; init; }
}