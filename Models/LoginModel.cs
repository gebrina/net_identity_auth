using System.ComponentModel.DataAnnotations;

namespace Net_Identity_Auth.Models;

public class LoginModel
{
    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string? EmailAddress { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Confirm password should match with password")]
    public string? ConfirmPassword { get; set; }

}