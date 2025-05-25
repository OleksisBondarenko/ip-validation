using ADValidation.Enums;
using ADValidation.Helpers.Ip;
using ADValidation.Models;
using ADValidation.Models.ERA;
using ADValidation.Services;

namespace ADValidation.Helpers.Validators;

public class PolicyValidator: Validator
{
    private readonly AccessPolicy _policy;
    
    public PolicyValidator(AccessPolicy policy)
    {
        _policy = policy;
    }
    
    public GeneralValidationResult<string?> ValidateIsComputerInWhitelist(string? ipAddress)
    {
        if (ipAddress == null || string.IsNullOrWhiteSpace(ipAddress))
        {
            return GeneralValidationResult<string?>.Fail(
                ipAddress,
                AuditType.NotFound,
                "IP address is null or computer info missing in whitelist."
            );
        }

        string[] allowedIpRules = _policy.IpFilterRules.ToArray();

            bool isIpAllowed = FirewallIpMatcher.IsIpInRule(ipAddress, allowedIpRules);
        
            return (isIpAllowed)
            ? GeneralValidationResult<string?>.Success(ipAddress, AuditType.AllowedByPolicy)
            : GeneralValidationResult<string?>.Fail(
                ipAddress,
                AuditType.NotFound,
                "IP address is not in whitelist."
            );
    }
    
}