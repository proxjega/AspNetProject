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
    public string? Name { get; set; }

    [Required]
    [StringLength(30)]
    public string? Surname { get; set; }

    [Range(1900, 9999)]
    public int DateOfBirth { get; set; }

    public GenderType? Gender { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(100)]
    public string? Address { get; set; }

    public List<string> UserNotes { get; set; } = new();
    
}