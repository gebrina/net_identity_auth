using Microsoft.AspNetCore.Mvc;
using Net_Identity_Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace Net_Identity_Auth.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class RoleController : ControllerBase
{
    private RoleManager<IdentityRole> _roleManager;
    private ILogger<RoleController> _logger;

    public RoleController(RoleManager<IdentityRole> roleManager, ILogger<RoleController> logger)
    {
        _roleManager = roleManager;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult GetAll()
    {
        var roles = _roleManager.Roles.Select(r => new { Id = r.Id, RoleName = r.Name });
        _logger.LogInformation("Fetching roles...");
        return Ok(roles);
    }

    [HttpPost]
    public async Task<ActionResult> Create(RoleModel roleModel)
    {
        if (!ModelState.IsValid) return BadRequest();
        IdentityRole role = new IdentityRole { Name = roleModel.RoleName };
        IdentityResult result = await _roleManager.CreateAsync(role);
        if (result.Succeeded) return CreatedAtAction(nameof(Create), new { Id = role.Id, RoleName = role.Name });

        foreach (IdentityError error in result.Errors)
        {
            ModelState.TryAddModelError(string.Empty, error.Description);
        }

        return BadRequest(ModelState);
    }
}