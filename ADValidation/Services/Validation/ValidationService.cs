using System.ComponentModel.DataAnnotations;
using System.Xml;
using ADValidation.Decorators;
using ADValidation.Enums;
using ADValidation.Helpers.Audit;
using ADValidation.Helpers.Ip;
using ADValidation.Helpers.OrderHelper;
using ADValidation.Helpers.Policy;
using ADValidation.Helpers.Validators;
using ADValidation.Models;
using ADValidation.Models.ERA;
using ADValidation.Services.Policy;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using ComputerInfo = ADValidation.Models.ComputerInfo;
using Validator = ADValidation.Helpers.Validators.Validator;

namespace ADValidation.Services.Validation;

public class ValidationService
{
    private readonly ERASettings _eraSettings;
    private readonly DomainService _domainService;
    private readonly EraService _eraService;
    private readonly LDAPSettings _ldapSettings;
    private readonly IMemoryCache _cache;

    private readonly ValidationSettings _validationSettings;
    // private readonly AuditLoggerService _auditLoggerService;
    private readonly AccessPolicyService _accessPolicyService;
    private readonly IPAddressService _ipAddressService;
    private readonly EraValidator _eraValidator;
    private readonly ILogger<ValidationService> _logger;

    private readonly TimeSpan _cacheDuration;


    public ValidationService(DomainService domainService, IMemoryCache cache, AuditLoggerService auditLoggerService,
        AccessPolicyService accessPolicyService, IPAddressService ipAddressService, EraService eraService,
        IOptions<ERASettings> eraSettings, IOptions<ValidationSettings> validationSettings,
        IOptions<LDAPSettings> ldapSettings)
    {
        _domainService = domainService;
        _eraService = eraService;
        _eraSettings = eraSettings.Value;
        _validationSettings = validationSettings.Value;
        _ldapSettings = ldapSettings.Value;
        _ipAddressService = ipAddressService;
        _accessPolicyService = accessPolicyService;
        // _auditLoggerService = auditLoggerService;
        _cache = cache;
        _eraValidator = new EraValidator(_domainService, _validationSettings);
        _cacheDuration = TimeSpan.FromMinutes(_validationSettings.CacheDurationMinutes);
    }

    public async Task<GeneralValidationResult<ComputerInfo>> GetComputerValidationResultAsync(string resource)
    {
        var validationResult = InitializeValidationResult(resource);

        try
        {
            //TODO: put all logic into policy.
            var policyResult = await _accessPolicyService.EvaluateIpAccessPolicy(validationResult.Data.IpAddress);
            if (policyResult.IsApplied)
            {
                validationResult = HandleValidationResultByPolicyResult(validationResult, policyResult);
            }

            var validationAgregatedResult =
                await GetComputerAggregatedValidationResult(validationResult.Data.IpAddress);

            validationResult =
                UpdateValidationResultWithAggregatedResult(validationResult, validationAgregatedResult.GeneralValidationResult);
         
            // _auditLoggerService.ExecuteWithAudit(validationResult);

            return validationResult;
        }
        catch (Exception e)
        {
        }

        return validationResult;
    }

    private GeneralValidationResult<ComputerInfo> HandleValidationResultByPolicyResult(GeneralValidationResult<ComputerInfo> validationResult,
        PolicyResult policyResult)
    {
        if (policyResult.Action == AccessAction.Allow)
        {
            validationResult.AuditType = AuditType.AllowedByPolicy;
            
        }
        else if (policyResult.Action == AccessAction.Deny)
        {
            validationResult.AuditType = AuditType.BlockedByPolicy;
        }

        // _auditLoggerService.ExecuteWithAudit(validationResult);
        return validationResult;
    }

    private GeneralValidationResult<ComputerInfo> UpdateValidationResultWithAggregatedResult(GeneralValidationResult<ComputerInfo> computerValidationResult, GeneralValidationResult<EraComputerInfo> eraValidationResult )
    {
        var computerData = computerValidationResult.Data;
        var eraData = eraValidationResult.Data;
        
        return new GeneralValidationResult<ComputerInfo>()
        {
            AuditType = eraValidationResult.AuditType,
            
            Data = new ComputerInfo () {
                IpAddress = computerData?.IpAddress ?? string.Empty,
                ResourceName = computerData?.ResourceName ?? string.Empty,
                Domain = eraData?.Domain ?? string.Empty,
                Hostname = eraData?.ComputerName ?? string.Empty,
                UserName = string.Empty,
            }
        }; 
    }

    private async Task<ValidationResultAgregated<EraComputerInfo>> GetComputerAggregatedValidationResult(
        string ipAddress)
    {
        ValidationResultAgregated<EraComputerInfo>? validationAgregatedResult = null;

        string cacheSuccessCompKey = $"comp_data_{ipAddress}";

        if (_cache.TryGetValue(cacheSuccessCompKey, out EraComputerInfo cachedData))
        {
            // validationAgregatedResult.Data = cachedData;
            var successValidationResult = GeneralValidationResult<EraComputerInfo>.Success(cachedData, AuditType.AllowedByCache);
            var successValidationResults = new List<GeneralValidationResult<EraComputerInfo>>()
                { successValidationResult };
            
            validationAgregatedResult =
                new ValidationResultAgregated<EraComputerInfo>(cachedData, successValidationResult,
                    successValidationResults);
            
            return validationAgregatedResult;
        }

        var validationResults = await ValidateWithEraAsync(ipAddress);


        var eraPriority = new List<AuditType>
        {
            AuditType.Ok,
            AuditType.NotFoundEset,
            AuditType.NotFoundDomain,
            AuditType.NotValidEsetTimespan,
            AuditType.NotFound,
        };

        var okFirstVariant =
            OrderValidationResultHelper.SelectValidationResultWithCustomPriority(validationResults, eraPriority);
        // validationResults.FirstOrDefault(data => data.IsValid) ??
        //                          validationResults.OrderByDescending(data => data.AuditType)
        //                              .FirstOrDefault(data => !data.IsValid);
        //

        var computerData = okFirstVariant?.Data ?? new EraComputerInfo { IpAddress = ipAddress };

        if (okFirstVariant?.IsValid == true)
        {
            _cache.Set(cacheSuccessCompKey, computerData, _cacheDuration);
        }

        return new ValidationResultAgregated<EraComputerInfo>(computerData, okFirstVariant, validationResults);
    }


    public async Task<List<GeneralValidationResult<EraComputerInfo>>> ValidateWithEraAsync(string ipAddress)
    {
        var tasks = _eraSettings.EraDbConnectionStrings.Select(connectionStringKV =>
            _eraService.GetComputerInfoSafe(connectionStringKV.Value, ipAddress)).ToList();

        var validators = new List<ValidationFunc<EraComputerInfo>>
        {
            _eraValidator.ValidateIsEraDbConnectionEstablished,
            _eraValidator.ValidateIsComputerFound,
            _eraValidator.ValidateIsComputerInDomain,
            _eraValidator.ValidateIsComputerInEset,
            _eraValidator.ValidateIsComputerHasValidEsetTimestamp
        };

        var results = await Validator.ValidateDataAsync<EraComputerInfo>(tasks, validators);

        return results;
    }


    private GeneralValidationResult<ComputerInfo> InitializeValidationResult(string? resource)
    {
        return new GeneralValidationResult<ComputerInfo>()
        {
            // IsValid = false,
            // ErrorMessage = "",
            AuditType = AuditType.NotFound,
            Data = new ComputerInfo () {
                IpAddress = _ipAddressService.ExtractIPv4(_ipAddressService.GetRequestIP()),
                ResourceName = string.IsNullOrEmpty(resource) ? string.Empty : resource
            }
        };
    }
}