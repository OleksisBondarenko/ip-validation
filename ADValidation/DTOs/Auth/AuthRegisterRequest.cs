using System.ComponentModel.DataAnnotations;

namespace ADValidation.DTOs.Auth;

public class AuthRegisterRequest
{
    [Required]
    public string Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
    
    // Add any additional user properties here
} 