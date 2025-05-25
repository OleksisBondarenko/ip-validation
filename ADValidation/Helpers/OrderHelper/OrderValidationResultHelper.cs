
using ADValidation.Enums;
using ADValidation.Helpers.Validators;

namespace ADValidation.Helpers.OrderHelper;

public class OrderValidationResultHelper
{
    public static GeneralValidationResult<T>? SelectValidationResultWithCustomPriority<T>(
        IEnumerable<GeneralValidationResult<T>> validationResults,
        List<AuditType> priorityOrder)
    {
        // Try to find the first valid result
        var validResult = validationResults.FirstOrDefault(v => v.IsValid);
        if (validResult != null)
            return validResult;

        // Fallback to custom-prioritized invalid result
        var prioritizedResult = validationResults
            .Where(v => !v.IsValid)
            .OrderByDescending(v => priorityOrder.IndexOf(v.AuditType))
            .FirstOrDefault();

        return prioritizedResult;
    }
}