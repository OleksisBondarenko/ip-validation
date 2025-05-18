using ADValidation.Enums;
using ADValidation.Models.Audit;

namespace ADValidation.DTOs.AccessPolicy;

public class AccessPolicyDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    
    public long Order { get; set; } = 0;
    public ICollection<string> IpFilterRules { get; set; } = new List<string>(); // By iprange and ricl mask

    public AccessAction Action { get; set; } // e.g., "Allow" or "Deny"

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime PolicyStartDatetime { get; set; }
    public DateTime PolicyEndDatetime { get; set; }

    public ICollection<AuditRecord> AuditRecords { get; set; } = new List<AuditRecord>();
}