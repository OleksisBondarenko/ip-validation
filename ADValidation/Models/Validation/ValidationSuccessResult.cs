namespace ADValidation.Models;

public class ValidationSuccessResult
{
    public string IpAddress { get; set; } = string.Empty;
    public string Hostname { get; set; } = string.Empty;
    public string ResourceName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}