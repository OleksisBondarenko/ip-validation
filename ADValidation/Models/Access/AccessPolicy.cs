using System.ComponentModel.DataAnnotations;
using ADValidation.Enums;
using ADValidation.Models.Audit;

namespace ADValidation.Models.Access;

public class AccessPolicy
{
    [Key]
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    
    public long Order { get; set; } = 0;
    public ICollection<string> IpFilterRules { get; set; } = new List<string>(); // By iprange and ricl mask
    public AccessAction Action { get; set; }    // e.g., "Allow" or "Deny"
    public string Resource { get; set; } = string.Empty;  // e.g., "it-d", "dpsu.gov.ua" 
    
    // List of validators to check access grant
    public ICollection<ValidatorType> ValidationTypes { get; set; } = new List<ValidatorType>();
    public ICollection<AuditRecord> AuditRecords { get; set; } = new List<AuditRecord>();
}