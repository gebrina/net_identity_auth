using System.ComponentModel.DataAnnotations;

namespace Net_Identity_Auth.Models;

public class RoleModel
{
    [Required(ErrorMessage = "Role name is required.")]
    public string? RoleName { get; set; }
}