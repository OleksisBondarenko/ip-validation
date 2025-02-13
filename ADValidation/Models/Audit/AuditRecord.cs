using System;

namespace ADValidation.Models.Audit;

public class AuditRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Data { get; set; } // JSON data
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}