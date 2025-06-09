using ADValidation.Enums;
using ADValidation.Helpers.Audit;

namespace ADValidation.DTOs.Audit;

public class AuditTypeInfo
{
    public AuditType Type { get; set; }
    public AuditTypeStatus Status => AuditTypeHelper.GetAuditTypeStatus(Type);
    public string Name => Type.ToString();
    public string DisplayName => AuditTypeHelper.GetAuditTypeStringForAdmin(Type);
    public bool IsAllow => AuditTypeHelper.GetAuditTypeAllowed(Type);
}