using System.ComponentModel.DataAnnotations;

namespace AspNetProject.DTOs;

public record RegisterDTO
{
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public required string Email { get; init; }

    [Required]
    [MinLength(6)]
    public required string password { get; init; }

    [Required]
    [StringLength(30)]
    public required string Name { get; init; }

    [Required]
    [StringLength(30)]
    public required string Surname { get; init; }



    
}