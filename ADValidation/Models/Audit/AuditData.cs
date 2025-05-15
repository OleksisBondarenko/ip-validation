using System.ComponentModel.DataAnnotations;

namespace ADValidation.Models.Audit;

public class AuditData
{
    [Key]
    public long Id { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string Hostname { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public long AuditRecordId { get; set; }
    public AuditRecord AuditRecord { get; set; } = null!;
}