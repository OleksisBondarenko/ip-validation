using ADValidation.Enums;

namespace ADValidation.Helpers.Policy;

public class PolicyResult
{
    public bool IsApplied { get; set; } = false;
    public AccessAction Action { get; set; } = AccessAction.Deny;
}