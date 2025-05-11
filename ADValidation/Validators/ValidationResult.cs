using ADValidation.Enums;

namespace ADValidation.Validators;

public class ValidationResult<T> 
{
    public bool IsValid { get; set; } = true;
    public string ErrorMessage { get; set; } = string.Empty;
    public AuditType AuditType { get; set; } = AuditType.Ok;
    public T? Data { get; set; } = default!;

    public ValidationResult ()
    {
        
    }
    
    public static ValidationResult<T> Success(T? data, AuditType auditType = AuditType.Ok) => new()
    {
        IsValid = true,
        Data = data,
        AuditType = auditType,
        ErrorMessage = string.Empty,
    };

    public static ValidationResult<T> Fail(T? data, AuditType auditType, string errorMessage) => new()
    {
        Data = data,
        IsValid = false,
        AuditType = auditType,
        ErrorMessage = errorMessage
    };
}