using System;
using System.ComponentModel.DataAnnotations;
using ADValidation.Enums;

namespace ADValidation.Models.Audit;

public class AuditRecord
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public AuditType AuditType { get; set; } = AuditType.NotFound;
    [StringLength(32)]
    public string ResourceName { get; set; } = string.Empty;
    public AuditData? AuditData { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}