namespace ADValidation.Helpers.Validators;

public delegate ValidationResult<TInput?> ValidationFunc<TInput>(TInput input);
