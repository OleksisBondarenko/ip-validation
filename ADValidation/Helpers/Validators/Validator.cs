using ADValidation.Enums;
using ADValidation.Helpers.Ip;
using ADValidation.Models;
using ADValidation.Models.ERA;
using ADValidation.Services;

namespace ADValidation.Helpers.Validators;

public class Validator
{
    public static async Task<List<GeneralValidationResult<T>>> ValidateDataAsync<T>(
        List<Task<T?>> dataTasks,
        List<ValidationFunc<T>> validators)
    {
        var results = new List<GeneralValidationResult<T>>();
        // Make a copy so we can remove as we go
        var pending = dataTasks.ToList();

        while (pending.Any())
        {
            // Wait for the next task to complete
            var completed = await Task.WhenAny(pending);
            pending.Remove(completed);

            // Get its result
            var data = await completed;

            // Validate
            var validationResult = GeneralValidationResult<T>.Success(data);
            foreach (var validator in validators)
            {
                validationResult = validator(data);
                if (!validationResult.IsValid)
                {
                    break;
                }
            }

            // If valid, bail out immediately:
            if (validationResult.IsValid)
            {
                return new List<GeneralValidationResult<T>> { validationResult };
            }

            // Otherwise collect the failure and continue
            results.Add(validationResult);
        }

        // No task produced a valid result
        return results;
    }
    
    public static bool ValidateData<T>(
        List<T?> dataList,
        List<ValidationFunc<T>> validators)
    {
        var results = new List<GeneralValidationResult<T>>();

        foreach (var data in dataList)
        {
            var validationResult = GeneralValidationResult<T>.Success(data);

            foreach (var validator in validators)
            {
                validationResult = validator(data);
                if (!validationResult.IsValid)
                {
                    break; // Stop further validation for this item
                }
            }

            // If one is valid, return immediately
            if (validationResult.IsValid)
            {
                results = new List<GeneralValidationResult<T>> { validationResult };
                return true;
            }

            // Otherwise collect failed validation result
            results.Add(validationResult);
        }

        return false;
    }

}
