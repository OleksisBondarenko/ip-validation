using ADValidation.Helpers.Validators;
using AutoMapper;

namespace ADValidation.Mappers.Validators;

public class ValidationResultMapper
{
    public static UserFriendlyValidationResult<T> MapToFriendly<T>(GeneralValidationResult<T> source)
    {
        return new UserFriendlyValidationResult<T> { AuditType = source.AuditType, Data = source.Data };
    } 
}