using System.ComponentModel.DataAnnotations;

namespace ADValidation.Models.Audit;

public class AuditData
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string IpAddress { get; set; } = String.Empty;
    public string Hostname { get; set; } = String.Empty;
    public string UserName { get; set; } = String.Empty;
    public string Domain { get; set; } = String.Empty;
    public Guid AuditRecordId { get; set; }
    public AuditRecord AuditRecord { get; set; } = null!;
}