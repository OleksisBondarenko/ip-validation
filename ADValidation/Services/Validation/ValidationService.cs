using ADValidation.Enums;
using ADValidation.Helpers.Ip;
using ADValidation.Models;
using ADValidation.Models.ERA;
using ADValidation.Validators;
using Microsoft.Extensions.Options;

namespace ADValidation.Services.Validation;

public class ValidationService
{
    private readonly ERASettings _eraSettings;
    private readonly DomainService _domainService;
    private readonly EraService _eraService;
    private readonly LDAPSettings _ldapSettings;
    private readonly ValidationSettings _validationSettings;
    private readonly Validator _validator;
    private readonly ILogger<ValidationService> _logger;


    public ValidationService(DomainService domainService,   EraService eraService, IOptions<ERASettings> eraSettings, IOptions<ValidationSettings> validationSettings, IOptions<LDAPSettings> ldapSettings)
    {
        _domainService = domainService;
        _eraService = eraService;
        _eraSettings = eraSettings.Value;
        _validationSettings = validationSettings.Value;
        _ldapSettings = ldapSettings.Value;
        _validator = new Validator(_domainService, _validationSettings);
    }

    public async Task<List<ValidationResult<EraComputerInfo>>> ValidateAsync(string ipAddress)
    {
        var tasks = _eraSettings.EraDbConnectionStrings.Select(connectionStringKV =>
            _eraService.GetComputerInfoSafe(connectionStringKV.Value, ipAddress)).ToList();
        
        var whiteListValidationResult = _validator.ValidateIsComputerInWhitelist(new EraComputerInfo() { IpAddress = ipAddress });
        if (whiteListValidationResult.IsValid)
        {
            return new List<ValidationResult<EraComputerInfo>> { whiteListValidationResult };
        }

        var validators = new List<ValidationFunc<EraComputerInfo>>
        {
            _validator.ValidateIsComputerFound,
            _validator.ValidateIsComputerInDomain,
            _validator.ValidateIsComputerInEset,
            _validator.ValidateIsComputerHasValidEsetTimestamp
        };
        
        var results = await Validator.ValidateDataAsync<EraComputerInfo>(tasks, validators);
        return results;
        //
        // foreach (var result in results)
        // {
        //     if (result.IsValid)
        //     {
        //        return result;
        //     }
        // }
        //
        // return ValidationResultWithData<EraComputerInfo>.Fail(AuditType.NotFound, "General error");
    }
    
   
}