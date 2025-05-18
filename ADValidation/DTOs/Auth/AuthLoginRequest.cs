using System.ComponentModel.DataAnnotations;

namespace ADValidation.DTOs.Auth;

public class AuthLoginRequest
{
    [Required]
    public string Username { get; set; }
    
    [Required]
    public string Password { get; set; }
}
