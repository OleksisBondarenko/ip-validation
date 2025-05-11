namespace ADValidation.Validators;

public delegate ValidationResult<T?> ValidationFunc<T>(T input);
