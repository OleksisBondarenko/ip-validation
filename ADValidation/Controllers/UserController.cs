using System.Security.Claims;
using ADValidation.DTOs.User;
using ADValidation.Models.Auth;
using ADValidation.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ADValidation.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class UserController: ControllerBase
{

    private readonly UserManager<ApplicationUser> _userManager;
    
    public UserController(UserManager<ApplicationUser> userManager)
    {
        this._userManager = userManager;
    }
    
    [HttpGet()]
    public async Task<IActionResult> UserDetailsById ()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        
        var user = await _userManager.FindByEmailAsync(email);
        
        try
        {       
            return Ok (new UserDetailedDto() { Email = user.Email, Username  = user.UserName, UserId = user.Id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}