using ADValidation.Models.Auth;
using ADValidation.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ADValidation.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        try
        {   
            var tokenResponse = await _authService.LoginAsync(model);
            return Ok(tokenResponse);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        try
        {
            var result = await _authService.RegisterAsync(model);
            
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { errors });
            }

            return Ok(new { message = "Registration successful" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("register")]
    public async Task<IActionResult> Register([FromQuery]string? email, [FromQuery]string? password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return BadRequest("Username or password is empty");
        }

        var registerModel = new RegisterModel()
        {
            Email = email,
            Password = password,
            ConfirmPassword = password
        };

        return await Register(registerModel);
    }

    // [HttpPost("refresh-token")]
    // public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    // {
    //     try
    //     {
    //         var tokenResponse = await _authService.RefreshTokenAsync(request);
    //         return Ok(tokenResponse);
    //     }
    //     catch (Exception ex)
    //     {
    //         return BadRequest(new { message = ex.Message });
    //     }
    // }

    // [HttpPost("revoke-token")]
    // [Authorize]
    // public async Task<IActionResult> RevokeToken([FromBody] string token)
    // {
    //     try
    //     {
    //         await _authService.RevokeTokenAsync(token);
    //         return Ok(new { message = "Token revoked" });
    //     }
    //     catch (Exception ex)
    //     {
    //         return BadRequest(new { message = ex.Message });
    //     }
    // }
}