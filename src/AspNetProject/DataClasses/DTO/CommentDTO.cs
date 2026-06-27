using System.ComponentModel.DataAnnotations;

namespace AspNetProject.DTOs;

public record CommentDTO
{
    public long Id { get; init; }

    [Required]
    [StringLength(200)]
    public required string Content { get; init; }

    public long PostId { get; init; }

    public long UserId { get; init; }
}