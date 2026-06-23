using System.ComponentModel.DataAnnotations;

namespace AspNetProject.Models;

public class UserDTO
{ 
    public long Id { get; set; }

    [Required]
    [StringLength(30)]
    public string? Name { get; set; }

    [Required]
    [StringLength(30)]
    public string? Surname { get; set; }

    public GenderType? Gender { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    
}