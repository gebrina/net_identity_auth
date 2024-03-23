using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Net_Identity_Auth.Models;

namespace Net_Identity_Auth.Controllers;

[ApiController]
[Route("/api[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<ApplicatoinUser> _signInManager;
    private readonly UserManager<ApplicatoinUser> _userManager;

    public AuthController(SignInManager<ApplicatoinUser> signInManager
    , UserManager<ApplicatoinUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet]
    public ActionResult Users()
    {
        var users = _userManager.Users.ToList();

        return Ok(users);
    }

    [HttpPost]
    public async Task<ActionResult> CreateUser(UserModel userModel)
    {
        if (!ModelState.IsValid) return BadRequest();

        ApplicatoinUser user = new ApplicatoinUser
        {
            FirstName = userModel.FirstName,
            LastName = userModel.LastName,
            UserName = userModel.EmailAddress,
            Email = userModel.EmailAddress,
            Occupation = userModel.Occupation,
        };

        IdentityResult result = await _userManager.CreateAsync(user, userModel.Password);
        if (result.Succeeded)
        {
            // login user 
            return Created(nameof(CreateUser), new { Message = "User created successfully." });
        }
        foreach (IdentityError error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return BadRequest(ModelState);
    }
}