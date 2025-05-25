namespace ADValidation.Helpers.Validators;

public delegate GeneralValidationResult<TInput?> ValidationFunc<TInput>(TInput input);
