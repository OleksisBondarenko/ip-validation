using ADValidation.Enums;
using ADValidation.Helpers.Audit;
using ADValidation.Models;

namespace ADValidation.Helpers.Validators;

public class UserFriendlyValidationResult<T>: GeneralValidationResult<T> 
{
    public string ErrorMessage
    {
        get
        {
            return AuditTypeHelper.GetAuditTypeStringForUser(AuditType);
        }
    }
}