using ADValidation.Enums;
using ADValidation.Helpers.Audit;
using ADValidation.Models;

namespace ADValidation.Helpers.Validators;

public class ADValidationResult<T> 
{
    public bool IsValid { get; set; } = true;
    public string ErrorMessage { get; set; } = string.Empty;
    public AuditType AuditType { get; set; } = AuditType.Ok;
    public T? Data { get; set; } = default!;
    
    public ADValidationResult ()
    {
        
    }
    
    public static ADValidationResult<T> Success(T? data, AuditType auditType = AuditType.Ok) => new()
    {
        IsValid = true,
        Data = data,
        AuditType = auditType,
        ErrorMessage = string.Empty,
    };

    public static ADValidationResult<T> Fail(T? data, AuditType auditType, string errorMessage) => new()
    {
        Data = data,
        IsValid = false,
        AuditType = auditType,
        ErrorMessage = errorMessage
    };
}