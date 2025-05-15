using System.Xml;
using ADValidation.Data;
using ADValidation.Enums;
using ADValidation.Helpers.Ip;
using ADValidation.Helpers.Validators;
using ADValidation.Models;
using ADValidation.Models.ERA;
using ADValidation.Models.Access;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ADValidation.Services.AccessPolicy;

public class AccessPolicyService 
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ValidationSettings _validationSettings;
    private List<Models.Access.AccessPolicy> _accessPolicies;
  
    
    public AccessPolicyService(
        ApplicationDbContext dbContext, 
        IOptions<ValidationSettings> validationSettings)
    {
        _dbContext = dbContext;
        _validationSettings = validationSettings.Value;
        _accessPolicies = new List<Models.Access.AccessPolicy>();
    }

    private async Task<List<Models.Access.AccessPolicy>> GetAccessPolicies()
    {
        if (!_accessPolicies.Any())
        {
            _accessPolicies = await _dbContext.AccessPolicies
                // .Where(policy => policy.IsActive)
                // .Where(policy => policy.IpFilterRules.Any())
                // .Where(policy => policy.Resource.IsNullOrEmpty())
                // .Where(policy => policy.ValidationTypes.Contains(ValidatorType.Era))
                .OrderBy(policy => policy.Order)
                .ToListAsync();
        }

        return _accessPolicies;
    }

    // Get all policies from DB
    public async Task<List<Models.Access.AccessPolicy>> GetAllPoliciesAsync()
    {
        return await _dbContext.AccessPolicies.ToListAsync();
    }

    public async Task<bool> ValidatePreData(string ipAddress)
    {

        throw new NotImplementedException();
    }
    
    public async Task<AccessAction> ValidateOnlyIpAddress(string ipAddress)
    {
        var policies = await GetAccessPolicies();
        policies.Add(PolicyFromConfig());
        
        foreach (var policy in policies)
        {
            // Skip if no IP filter rules
            if (policy.IpFilterRules != null && policy.IpFilterRules.Any())
            {
                bool ipAllowed = FirewallIpMatcher.IsIpAllowed(ipAddress, policy.IpFilterRules.ToArray());

                if (!ipAllowed)
                    continue; // IP not allowed by this policy
            }

            // IP is allowed → apply the policy's action
            return policy.Action;
        }

        // No matching policy found → deny by default
        return AccessAction.Deny;
    }

    private Models.Access.AccessPolicy PolicyFromConfig ()
    {
        var whiteListReader = new WhiteListIpConfigReader(_validationSettings.WhiteListConfigPath);
        var whiteListIps = whiteListReader.WhiteListIPs();

        return new Models.Access.AccessPolicy()
        {
            Action = AccessAction.Allow,
            Order = 1,
            IpFilterRules = whiteListIps,
            IsActive = true,
            ValidationTypes = new List<ValidatorType> { ValidatorType.Ip },
        };
    }
    
    private bool IsWhiteListIp(string ip)
    {
        var whiteListReader = new WhiteListIpConfigReader(_validationSettings.WhiteListConfigPath);
        var whiteListIps = whiteListReader.WhiteListIPs();
        return whiteListIps.Contains(ip);
    }

}

