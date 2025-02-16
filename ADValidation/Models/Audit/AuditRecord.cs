using System;

namespace ADValidation.Models.Audit;

public class AuditRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public AuditData AuditData { get; set; }
    public Guid AuditDataId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}