using ADValidation.Enums;
using ADValidation.Helpers.Ip;
using ADValidation.Models;
using ADValidation.Models.ERA;
using ADValidation.Services;

namespace ADValidation.Validators;

public class Validator
{
    private readonly ValidationSettings _validationSettings;
    private readonly DomainService _domainService;

    public static async Task<List<ValidationResult<T>>> ValidateDataAsync<T>(
        List<Task<T?>> dataTasks,
        List<ValidationFunc<T>> validators)
    {
        var results = new List<ValidationResult<T?>>();

        foreach (var task in dataTasks)
        {
            var data = await task;
            
            // by default consider that validation result is valid and has not null data.
            var lastValidationResult = ValidationResult<T?>.Success(data);
            
            foreach (var validator in validators)
            {
                var result = validator(data);
                if (!result.IsValid)
                    lastValidationResult = result;
            }
            
            results.Add(lastValidationResult);
        }

        // TODO: change to valid.
        return results;
    }
    
    public Validator(DomainService domainService, ValidationSettings validationSettings)
    {
        _domainService = domainService;
        _validationSettings = validationSettings;
    }

    public ValidationResult<EraComputerInfo?> ValidateIsComputerInWhitelist(EraComputerInfo? computerInfo)
    {
        if (computerInfo == null || string.IsNullOrWhiteSpace(computerInfo.IpAddress))
        {
            return ValidationResult<EraComputerInfo?>.Fail(
                computerInfo,
                AuditType.NotFound,
                "IP address is null or computer info missing in whitelist."
            );
        }

        return IsWhiteListIp(computerInfo.IpAddress)
            ? ValidationResult<EraComputerInfo?>.Success(computerInfo, AuditType.OkWhiteListIp)
            : ValidationResult<EraComputerInfo?>.Fail(
                computerInfo,
                AuditType.NotFound,
                "IP address is not in whitelist."
            );
    }

    public ValidationResult<EraComputerInfo?> ValidateIsComputerFound(EraComputerInfo? computerInfo)
    {
        return computerInfo == null
            ? ValidationResult<EraComputerInfo?>.Fail(
                computerInfo, 
                AuditType.NotFoundEset,
                "Computer not found in ESET ERA Database")
            : ValidationResult<EraComputerInfo?>.Success(computerInfo);
    }

    public ValidationResult<EraComputerInfo?> ValidateIsComputerInDomain(EraComputerInfo? computerInfo)
    {
        if (computerInfo == null)
        {
            return ValidateIsComputerFound(computerInfo);
        }

        var domain = _domainService.GetDomainFromHostname(computerInfo.ComputerName);
        computerInfo.Domain = domain;
        
        return string.IsNullOrWhiteSpace(domain)
            ? ValidationResult<EraComputerInfo?>.Fail(
                computerInfo,
                AuditType.NotFoundDomain,
                "Computer name in ERA has invalid domain suffix."
            )
            : ValidationResult<EraComputerInfo?>.Success(computerInfo);
    }

    public ValidationResult<EraComputerInfo?> ValidateIsComputerInEset(EraComputerInfo? computerInfo)
    {
        return computerInfo == null
            ? ValidateIsComputerFound(computerInfo)
            : ValidationResult<EraComputerInfo?>.Success(computerInfo);
    }

    public ValidationResult<EraComputerInfo?> ValidateIsComputerHasValidEsetTimestamp(EraComputerInfo? computerInfo)
    {
        if (computerInfo == null)
        {
            return ValidateIsComputerFound(computerInfo);
        }

        int eraTimespanMilis = _validationSettings.EsetValidConnectionTimespan;
        var allowedTime = computerInfo.ComputerConnected.AddMilliseconds(eraTimespanMilis);
        bool isValidTimestamp = allowedTime >= DateTime.Now;

        return isValidTimestamp
            ? ValidationResult<EraComputerInfo?>.Success(computerInfo)
            : ValidationResult<EraComputerInfo?>.Fail(
                    computerInfo,
                AuditType.NotValidEsetTimespan,
                $"Computer wasn't online in ESET ERA more than {TimeSpan.FromMilliseconds(eraTimespanMilis)} ago."
            );
    }

    private bool IsWhiteListIp(string ip)
    {
        var whiteListReader = new WhiteListIpConfigReader(_validationSettings.WhiteListConfigPath);
        var whiteListIps = whiteListReader.WhiteListIPs();
        return whiteListIps.Contains(ip);
    }
}
