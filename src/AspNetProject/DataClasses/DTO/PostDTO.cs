using System.ComponentModel.DataAnnotations;

namespace AspNetProject.DTOs;

public record PostDTO
{
    public long Id { get; init; }

    [Required]
    [StringLength(100)]
    public required string Title { get; init; }

    public string? Content { get; init; }

    public long UserId { get; init; }
}