using ADValidation.Enums;
using ADValidation.Helpers.Ip;
using ADValidation.Helpers.Validators;
using ADValidation.Models;
using ADValidation.Models.ERA;
using ADValidation.Services.Policy;
using Microsoft.Extensions.Options;

namespace ADValidation.Services.Validation;

public class ValidationService
{
    private readonly ERASettings _eraSettings;
    private readonly DomainService _domainService;
    private readonly EraService _eraService;
    private readonly LDAPSettings _ldapSettings;
    private readonly AccessPolicyService _accessPolicy;
    private readonly ValidationSettings _validationSettings;
    private readonly EraValidator _eraValidator;
    private readonly ILogger<ValidationService> _logger;
    
    public ValidationService(DomainService domainService,  AccessPolicyService accessPolicy,  EraService eraService, IOptions<ERASettings> eraSettings, IOptions<ValidationSettings> validationSettings, IOptions<LDAPSettings> ldapSettings)
    {
        _domainService = domainService;
        _eraService = eraService;
        _accessPolicy = accessPolicy;
        _eraSettings = eraSettings.Value;
        _validationSettings = validationSettings.Value;
        _ldapSettings = ldapSettings.Value;
        _eraValidator = new EraValidator(_domainService, _validationSettings);
    }

    public async Task<List<ValidationResult<EraComputerInfo>>> ValidateWithEraAsync(string ipAddress)
    {
        var tasks = _eraSettings.EraDbConnectionStrings.Select(connectionStringKV =>
            _eraService.GetComputerInfoSafe(connectionStringKV.Value, ipAddress)).ToList();
        
        var validators = new List<ValidationFunc<EraComputerInfo>>
        {
            _eraValidator.ValidateIsComputerFound,
            _eraValidator.ValidateIsComputerInDomain,
            _eraValidator.ValidateIsComputerInEset,
            _eraValidator.ValidateIsComputerHasValidEsetTimestamp
        };
        
        var results = await Validator.ValidateDataAsync<EraComputerInfo>(tasks, validators);
        
        return results;
    }
    
   
}