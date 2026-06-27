using System.ComponentModel.DataAnnotations;

namespace AspNetProject.Models;

public class Comment
{
    public long Id { get; set; }

    [Required]
    [StringLength(200)]
    public required string Content { get; set; }

    public long PostId { get; set; }
    public Post Post { get; set;} = null!;

    public long UserId { get; set; }
    public User User { get; set; } = null!;
}