namespace ADValidation.DTOs.Auth;

public class RefreshToken
{
    public Guid Id { get; set; }
    
    public string Token { get; set; } = string.Empty;
}