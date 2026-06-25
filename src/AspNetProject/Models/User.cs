using System.ComponentModel.DataAnnotations;

namespace AspNetProject.Models;

public enum GenderType
{
    Male,
    Female,
    NonBinary,
}

public class User
{ 
    public long Id { get; set; }

    [Required]
    [StringLength(30)]
    public required string Name { get; set; }

    [Required]
    [StringLength(30)]
    public required string Surname { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public GenderType? Gender { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public required string Email { get; set; }

    [StringLength(100)]
    public string? Address { get; set; }

    public List<string> UserNotes { get; set; } = new();

    public bool IsAdmin {get; set; } = false;

    public bool IsVerified {get; set; } = false;

    public ICollection<Post> Posts { get; } = new List<Post>();

    public ICollection<Comment> Comments { get; } = new List<Comment>();

}