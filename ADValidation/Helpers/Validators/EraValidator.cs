using ADValidation.Enums;
using ADValidation.Helpers.Ip;
using ADValidation.Models;
using ADValidation.Models.ERA;
using ADValidation.Services;

namespace ADValidation.Helpers.Validators;

public class EraValidator: Validator
{
    private readonly ValidationSettings _validationSettings;
    private readonly DomainService _domainService;
    
    public EraValidator(DomainService domainService, ValidationSettings validationSettings)
    {
        _domainService = domainService;
        _validationSettings = validationSettings;
    }
    
    // public ValidationResult<EraComputerInfo?> ValidateIsComputerInWhitelist(EraComputerInfo? computerInfo)
    // {
    //     if (computerInfo == null || string.IsNullOrWhiteSpace(computerInfo.IpAddress))
    //     {
    //         return ValidationResult<EraComputerInfo?>.Fail(
    //             computerInfo,
    //             AuditType.NotFound,
    //             "IP address is null or computer info missing in whitelist."
    //         );
    //     }
    //
    //     return IsWhiteListIp(computerInfo.IpAddress)
    //         ? ValidationResult<EraComputerInfo?>.Success(computerInfo, AuditType.OkWhiteListIp)
    //         : ValidationResult<EraComputerInfo?>.Fail(
    //             computerInfo,
    //             AuditType.NotFound,
    //             "IP address is not in whitelist."
    //         );
    // }
    

    public GeneralValidationResult<EraComputerInfo?> ValidateIsEraDbConnectionEstablished(EraComputerInfo? computerInfo)
    {
        return computerInfo == null
            ? GeneralValidationResult<EraComputerInfo?>.Fail(
                computerInfo, 
                AuditType.NoAccessToDb,
                "ESET ERA Database is disabled...")
            : GeneralValidationResult<EraComputerInfo?>.Success(computerInfo);
    }
    
    public GeneralValidationResult<EraComputerInfo?> ValidateIsComputerFound(EraComputerInfo? computerInfo)
    {
        return computerInfo != null && computerInfo.ComputerId == 0
            ? GeneralValidationResult<EraComputerInfo?>.Fail(
                computerInfo, 
                AuditType.NotFoundEset,
                "Computer not found in ESET ERA Database")
            : GeneralValidationResult<EraComputerInfo?>.Success(computerInfo);
    }

    public GeneralValidationResult<EraComputerInfo?> ValidateIsComputerInDomain(EraComputerInfo? computerInfo)
    {
        if (computerInfo == null)
        {
            return ValidateIsComputerFound(computerInfo);
        }

        var domain = _domainService.GetDomainFromHostname(computerInfo.ComputerName);
        computerInfo.Domain = domain;
        
        return string.IsNullOrWhiteSpace(domain)
            ? GeneralValidationResult<EraComputerInfo?>.Fail(
                computerInfo,
                AuditType.NotFoundDomain,
                "Computer name in ERA has invalid domain suffix."
            )
            : GeneralValidationResult<EraComputerInfo?>.Success(computerInfo);
    }

    public GeneralValidationResult<EraComputerInfo?> ValidateIsComputerInEset(EraComputerInfo? computerInfo)
    {
        return computerInfo == null
            ? ValidateIsComputerFound(computerInfo)
            : GeneralValidationResult<EraComputerInfo?>.Success(computerInfo);
    }

    public GeneralValidationResult<EraComputerInfo?> ValidateIsComputerHasValidEsetTimestamp(EraComputerInfo? computerInfo)
    {
        if (computerInfo == null)
        {
            return ValidateIsComputerFound(computerInfo);
        }

        int eraTimespanMilis = _validationSettings.EsetValidConnectionTimespan;
        var allowedTime = computerInfo.ComputerConnected.AddMilliseconds(eraTimespanMilis);
        bool isValidTimestamp = allowedTime >= DateTime.Now;

        return isValidTimestamp
            ? GeneralValidationResult<EraComputerInfo?>.Success(computerInfo)
            : GeneralValidationResult<EraComputerInfo?>.Fail(
                    computerInfo,
                AuditType.NotValidEsetTimespan,
                $"Computer wasn't online in ESET ERA more than {TimeSpan.FromMilliseconds(eraTimespanMilis)} ago."
            );
    }

 
}