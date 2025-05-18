using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ADValidation.Data;
using ADValidation.DTOs.Auth;
using ADValidation.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using LoginRequest = Microsoft.AspNetCore.Identity.Data.LoginRequest;

namespace ADValidation.Services.Auth;

public class AuthService 
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _context = context;
    }

    public async Task<TokenResponse> LoginAsync(AuthLoginRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Username);
        if (user == null)
            throw new Exception("User not found");
        
        var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!passwordValid) 
            throw new Exception("Invalid password");
        
        //await _signInManager.SignInAsync(user, isPersistent: false);
        
        
        // if (!result.Succeeded)
        // {
        //     var errors = new {
        //         Result = result.ToString(),
        //         UserExists = user != null,
        //         EmailConfirmed = user?.EmailConfirmed,
        //         LockoutEnabled = user?.LockoutEnabled,
        //         LockoutEnd = user?.LockoutEnd
        //     };
        //     throw new Exception($"Login failed: {JsonSerializer.Serialize(errors)}");
        // }
        var token = await GenerateJwtToken(user);
        return token;
    }

    public async Task<IdentityResult> RegisterAsync(AuthRegisterRequest request)
    {
        var user = new ApplicationUser()
        {
            UserName = request.Email,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        return result;
    }

    // public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
    // {
    //     var principal = GetPrincipalFromExpiredToken(request.AccessToken);
    //     var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier));
    //     
    //     var user = await _userManager.FindByIdAsync(userId.ToString());
    //     if (user == null)
    //         throw new Exception("User not found");
    //
    //     var storedRefreshToken = await _context.UserTokens
    //         .FirstOrDefaultAsync(rt => rt. == request.RefreshToken && rt.UserId == userId);
    //     
    //     if (storedRefreshToken == null || storedRefreshToken.ExpiryDate < DateTime.UtcNow)
    //         throw new Exception("Invalid refresh token");
    //
    //     // Generate new tokens
    //     var newToken = await GenerateJwtToken(user);
    //     
    //     // Remove old refresh token
    //     _context.RefreshTokens.Remove(storedRefreshToken);
    //     await _context.SaveChangesAsync();
    //
    //     return newToken;
    // }
    //
    // public async Task RevokeTokenAsync(string token)
    // {
    //     var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
    //     if (refreshToken != null)
    //     {
    //         _context.RefreshTokens.Remove(refreshToken);
    //         await _context.SaveChangesAsync();
    //     }
    // }

    private async Task<TokenResponse> GenerateJwtToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["AccessTokenExpirationMinutes"]));

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );
        
        // var refreshToken = new RefreshToken
        // {
        //     UserId = user.Id,
        //     Token = GenerateRefreshToken(),
        //     ExpiryDate = DateTime.UtcNow.AddDays(Convert.ToDouble(jwtSettings["RefreshTokenExpirationDays"])),
        //     CreatedDate = DateTime.UtcNow
        // };
        //
        // await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        return new TokenResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            // RefreshToken = refreshToken.Token,
            Expiration = expires
        };
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        
        if (securityToken is not JwtSecurityToken jwtSecurityToken || 
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
}