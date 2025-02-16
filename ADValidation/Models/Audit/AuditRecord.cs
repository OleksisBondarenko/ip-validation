using System;
using System.ComponentModel.DataAnnotations;

namespace ADValidation.Models.Audit;

public class AuditRecord
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public AuditData? AuditData { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}