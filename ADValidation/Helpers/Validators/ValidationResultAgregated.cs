using System.Xml;
using ADValidation.Enums;
using Microsoft.IdentityModel.Tokens;

namespace ADValidation.Helpers.Validators;

public class ValidationResultAgregated <T>
{
    public T Data { get; set; } = default!;
    public ValidationResult<T>?  ValidationResult { get; set; }
    public List<ValidationResult<T>>? ValidationResults { get; set; }
    
    public ValidationResultAgregated(
        T data, 
        ValidationResult<T> validationResult = null,
        List<ValidationResult<T>>? validationResults = null)
    {
        Data = data;
        ValidationResult = validationResult;
        ValidationResults = validationResults ?? new List<ValidationResult<T>>();
    }
}