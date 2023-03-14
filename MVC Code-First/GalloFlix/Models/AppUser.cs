using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GalloFlix.Models;

public class AppUser : IdentityUser
{
    [Required]
    [StringLength(60)]
    public string Name { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }
}
