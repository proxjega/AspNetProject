using System.ComponentModel.DataAnnotations;

namespace AspNetProject.Models;

public class CommentDTO
{
    public long Id { get; init; }

    [Required]
    [StringLength(200)]
    public required string Content { get; init; }

    public long PostId { get; init; }

    public long UserId { get; init; }
}