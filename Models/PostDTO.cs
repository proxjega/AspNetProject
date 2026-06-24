using System.ComponentModel.DataAnnotations;

namespace AspNetProject.Models;

public class PostDTO
{
    public long Id { get; set; }

    [Required]
    [StringLength(100)]
    public string? Title { get; set; }

    public string? Content { get; set; }

    public long UserId { get; set; }
}