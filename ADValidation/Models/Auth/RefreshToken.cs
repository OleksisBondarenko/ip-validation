namespace ADValidation.Models.Auth;

public class RefreshToken
{
    public Guid Id { get; set; }
    
    public string Token { get; set; } = string.Empty;
}