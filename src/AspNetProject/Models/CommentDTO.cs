using System.ComponentModel.DataAnnotations;

namespace AspNetProject.Models;

public class CommentDTO
{
    public long Id { get; set; }

    [Required]
    [StringLength(200)]
    public required string Content { get; set; }

    public long PostId { get; set; }

    public long UserId { get; set; }
}