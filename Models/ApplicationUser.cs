using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Net_Identity_Auth.Models;

public class ApplicatoinUser : IdentityUser
{
    [Required]
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Occupation { get; set; }
}