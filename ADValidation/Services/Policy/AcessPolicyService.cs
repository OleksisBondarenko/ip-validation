using System.Xml;
using ADValidation.Data;
using ADValidation.Enums;
using ADValidation.Helpers.Ip;
using ADValidation.Helpers.Policy;
using ADValidation.Helpers.Validators;
using ADValidation.Models;
using ADValidation.Models.ERA;
using ADValidation.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ADValidation.Services.Policy;

public class AccessPolicyService 
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ValidationSettings _validationSettings;
    private List<AccessPolicy> _accessPolicies;
  
    
    public AccessPolicyService(
        ApplicationDbContext dbContext, 
        IOptions<ValidationSettings> validationSettings
        )
    {
        _dbContext = dbContext;
        _validationSettings = validationSettings.Value;
        _accessPolicies = new List<AccessPolicy>();
    }

    private async Task<List<AccessPolicy>> GetActivePolicies()
    {
        if (!_accessPolicies.Any())
        {
            _accessPolicies = await _dbContext.AccessPolicies
                .Where(policy => policy.IsActive)
                // .Where(policy => policy.IpFilterRules.Any())
                // .Where(policy => policy.Resource.IsNullOrEmpty())
                // .Where(policy => policy.ValidationTypes.Contains(ValidatorType.Era))
                .OrderBy(policy => policy.Order)
                .ToListAsync();
        }

        return _accessPolicies;
    }

    // Get all policies from DB
    public async Task<List<AccessPolicy>> GetAllPoliciesAsync()
    {
        return await _dbContext.AccessPolicies.ToListAsync();
    }

    public async Task<bool> ValidatePreData(string ipAddress)
    {

        throw new NotImplementedException();
    }
    
    public async Task<PolicyResult> EvaluateIpAccessPolicy(string ipAddress)
    {
        PolicyResult result = new PolicyResult();
        var policies = await GetActivePolicies();
        
        foreach (var policy in policies)
        {
            // var policy = policies[i];
            // Skip if no IP filter rules
            if (policy.IpFilterRules != null && policy.IpFilterRules.Any())
            {
                bool ipInRule = FirewallIpMatcher.IsIpInRule(ipAddress, policy.IpFilterRules.ToArray());
        
                if (ipInRule)
                {
                    result.IsApplied = true;
                    result.Action = policy.Action;
                    break;
                } 
                else
                    continue; // IP not allowed by this policy
            }
        }

        return result;
    }
    
    private bool IsWhiteListIp(string ip)
    {
        var whiteListReader = new WhiteListIpConfigReader(_validationSettings.WhiteListConfigPath);
        var whiteListIps = whiteListReader.WhiteListIPs();
        return whiteListIps.Contains(ip);
    }

}

