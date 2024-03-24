using System.ComponentModel.DataAnnotations;

namespace Net_Identity_Auth.Models;

public class UpdateUserModel
{

    public string? Id { get; set; }

    [Required]
    public string? FirstName { get; set; }

    [Required]
    public string? LastName { get; set; }

    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string? EmailAddress { get; set; }

    public string? Occupation { get; set; }

    public ICollection<RoleModel>? Roles { get; set; }
}