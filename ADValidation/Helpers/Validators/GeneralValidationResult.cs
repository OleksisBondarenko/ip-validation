using ADValidation.Enums;
using ADValidation.Helpers.Audit;
using ADValidation.Models;

namespace ADValidation.Helpers.Validators;

public class GeneralValidationResult<T> 
{
    public bool IsValid
    {
        get
        {
            return AuditTypeHelper.GetAuditTypeAllowed(AuditType);
        }
    }

    public string ErrorMessage
    {
        get
        {
            return AuditTypeHelper.GetAuditTypeString(AuditType);
        }
    }

    public AuditType AuditType { get; set; } = AuditType.Ok;
    public T? Data { get; set; } = default!;
    
    public GeneralValidationResult ()
    {
        
    }
    
    public static GeneralValidationResult<T> Success(T? data, AuditType auditType = AuditType.Ok) => new()
    {
        // IsValid = true,
        Data = data,
        AuditType = auditType,
        // ErrorMessage = string.Empty,
    };

    public static GeneralValidationResult<T> Fail(T? data, AuditType auditType, string errorMessage) => new()
    {
        Data = data,
        // IsValid = false,
        AuditType = auditType,
        // ErrorMessage = errorMessage
    };
}