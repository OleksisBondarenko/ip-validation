using System;
using System.ComponentModel.DataAnnotations;
using ADValidation.Enums;
using ADValidation.Models.Access;

namespace ADValidation.Models.Audit;

public class AuditRecord
{   
    [Key]
    public long Id { get; set; }
    public AuditType AuditType { get; set; } = AuditType.NotFound;
    [StringLength(32)]
    public string ResourceName { get; set; } = string.Empty;
    public AuditData? AuditData { get; set; }   
    public ICollection<AccessPolicy> AccessPolicies { get; set; } = new List<AccessPolicy>();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}