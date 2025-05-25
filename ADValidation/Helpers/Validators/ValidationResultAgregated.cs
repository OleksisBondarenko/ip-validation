using System.Xml;
using ADValidation.Enums;
using Microsoft.IdentityModel.Tokens;

namespace ADValidation.Helpers.Validators;

public class ValidationResultAgregated <T>
{
    public T Data { get; set; } = default!;
    public GeneralValidationResult<T>?  GeneralValidationResult { get; set; }
    public List<GeneralValidationResult<T>>? ValidationResults { get; set; }
    
    public ValidationResultAgregated(
        T data, 
        GeneralValidationResult<T> generalValidationResult = null,
        List<GeneralValidationResult<T>>? validationResults = null)
    {
        Data = data;
        GeneralValidationResult = generalValidationResult;
        ValidationResults = validationResults ?? new List<GeneralValidationResult<T>>();
    }
}