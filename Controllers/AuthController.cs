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
    public async Task<ActionResult> Users()
    {
        var users = _userManager.Users.ToList();
        IList<UserModel> userModels = new List<UserModel>();
        foreach (var user in users)
        {
            var userWithRoles = await AddRoleToUser(IntializeUserModel(user));
            userModels.Add(userWithRoles);
        }
        var usersWithOutPassword = userModels.Select(user => RemovePassword(user));
        return Ok(usersWithOutPassword);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult> GetUserById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        var userWithRoles = await AddRoleToUser(IntializeUserModel(user));
        var userWithOutPassword = RemovePassword(userWithRoles);
        return Ok(userWithOutPassword);
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

    [NonAction]
    public UserModel IntializeUserModel(ApplicatoinUser user)
    {
        UserModel userModel = new UserModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Occupation = user.Occupation,
            EmailAddress = user.Email,
            Roles = new List<RoleModel>()
        };

        return userModel;
    }
    [NonAction]
    public object RemovePassword(UserModel userModel)
    {
        var user = new
        {
            Id = userModel.Id,
            FirstName = userModel.FirstName,
            LastName = userModel.LastName,
            EmailAddress = userModel.EmailAddress,
            Occupation = userModel.Occupation,
            Roles = userModel.Roles
        };
        return user;
    }
}