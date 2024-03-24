using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Net_Identity_Auth.Models;

namespace Net_Identity_Auth.Controllers;

[ApiController]
[Route("/api[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<ApplicatoinUser> _signInManager;
    private readonly UserManager<ApplicatoinUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthController(SignInManager<ApplicatoinUser> signInManager
    , UserManager<ApplicatoinUser> userManager
    , RoleManager<IdentityRole> roleManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    public ActionResult Users()
    {
        var users = _userManager.Users.ToList().Select(
            user => new UserModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.Email,
                Occupation = user.Occupation,
            }
        );

        return Ok(users);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult> GetUserById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        UserModel userModel = new UserModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Occupation = user.Occupation,
            EmailAddress = user.Email,
            Roles = new List<RoleModel>()
        };
        var userWithRoles = await AddRoleToUser(userModel);

        return Ok(userWithRoles);
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
            if (userModel?.Roles?.Count() > 0)
            {
                foreach (RoleModel role in userModel.Roles)
                {
                    await _userManager.AddToRoleAsync(user, role.RoleName);
                }
            }
            // login user 
            return Created(nameof(CreateUser), new { Message = "User created successfully." });
        }
        foreach (IdentityError error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return BadRequest(ModelState);
    }

    [NonAction]
    public async Task<UserModel> AddRoleToUser(UserModel userModel)
    {
        if (userModel.Id == null) throw new ArgumentNullException();
        var roles = _roleManager.Roles.ToList();
        var user = await _userManager.FindByIdAsync(userModel.Id);
        foreach (var role in roles)
        {
            if (await _userManager.IsInRoleAsync(user, role.Name) && userModel.Roles != null)
            {
                userModel.Roles.Add(new RoleModel { RoleName = role.Name });
            }
        }
        return userModel;
    }
}