using ADValidation.Enums;

namespace ADValidation.DTOs.AccessPolicy;

public class AccessPolicyDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public ICollection<string> IpFilterRules { get; set; } = new List<string>();
    public AccessAction Action { get; set; }
    public string Resource { get; set; } = string.Empty;
    public ICollection<ValidatorType> Validators { get; set; } = new List<ValidatorType>();
}