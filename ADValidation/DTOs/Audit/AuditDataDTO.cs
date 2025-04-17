namespace ADValidation.DTOs.Audit;

public class AuditDataDTO
{
    public string IpAddress { get; set; } = String.Empty;
    public string HostName { get; set; } = String.Empty;
    public string UserName { get; set; } = String.Empty;
    public string Domain { get; set; } = String.Empty;
}