using System.ComponentModel.DataAnnotations;

namespace AspNetProject.Models;

public class Post
{
    public long Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string Title { get; set; }

    public string? Content { get; set; }

    public long UserId { get; set; }
    public User User { get; set;} = null!;

    public ICollection<Comment> Comments { get; } = new List<Comment>();
}