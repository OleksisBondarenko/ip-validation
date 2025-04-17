using System.ComponentModel.DataAnnotations;

namespace ADValidation.Models.Audit;

public class AuditData
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string IpAddress { get; set; } = string.Empty;
    public string Hostname { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Guid AuditRecordId { get; set; }
    public AuditRecord AuditRecord { get; set; } = null!;
}