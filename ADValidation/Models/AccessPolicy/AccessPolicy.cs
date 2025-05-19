using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using ADValidation.Enums;
using ADValidation.Models.Audit;

namespace ADValidation.Models
{

    public class AccessPolicy
    {
        [Key] public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public long Order { get; set; } = 0;
        public ICollection<string> IpFilterRules { get; set; } = new List<string>(); // By iprange and ricl mask

        public AccessAction Action { get; set; } // e.g., "Allow" or "Deny"

        // public string Resource { get; set; } = string.Empty;  // e.g., "it-d", "dpsu.gov.ua" 
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime PolicyStartDatetime { get; set; }
        public DateTime PolicyEndDatetime { get; set; }

        // List of validators to check access grant
        // public ICollection<ValidatorType> ValidationTypes { get; set; } = new List<ValidatorType>();
        public ICollection<AuditRecord> AuditRecords { get; set; } = new List<AuditRecord>();
    }
}