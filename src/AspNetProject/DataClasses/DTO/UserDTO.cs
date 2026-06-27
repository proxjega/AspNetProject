using System.ComponentModel.DataAnnotations;
using AspNetProject.Models;

namespace AspNetProject.DTOs;


public record UserDTO
{ 
    public long Id { get; init; }

    [Required]
    [StringLength(30)]
    public required string Name { get; init; }

    [Required]
    [StringLength(30)]
    public required string Surname { get; init; }

    public DateOnly? DateOfBirth { get; init; }

    public GenderType? Gender { get; init; }

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public required string Email { get; init; }

    [StringLength(100)]
    public string? Address { get; init; }

    public List<string> UserNotes { get; init; } = new();
}