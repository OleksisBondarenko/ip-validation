using System.ComponentModel.DataAnnotations;
using ADValidation.Decorators;
using ADValidation.DTOs.AccessPolicy;
using ADValidation.Enums;
using ADValidation.Helpers.OrderHelper;
using ADValidation.Helpers.Validators;
using ADValidation.Models;
using ADValidation.Models.ERA;
using ADValidation.Services;
using ADValidation.Services.Policy;
using ADValidation.Services.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ADValidation.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ValidateController : ControllerBase
{
    private readonly AuditLoggerService _auditLogger;
    private readonly IPAddressService _ipAddressService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ValidateController> _logger;
    private readonly EraService _eraService;
    private readonly DomainService _domainService;
    private readonly TimeSpan _cacheDuration;
    private readonly ValidationSettings _validationSettings;
    private readonly ValidationService _validationService;
    private readonly AccessPolicyService _accessPolicyService;
    public ValidateController(
        IPAddressService ipAddressService,
        AuditLoggerService auditLogger,
        ValidationService validationService,
        AccessPolicyService accessPolicyService,
        IMemoryCache memoryCache,
        DomainService domainService,
        EraService eraService,
        ILogger<ValidateController> logger,
        IOptions<ValidationSettings> validationSettings
    )
    {

        _validationService = validationService;
        _auditLogger = auditLogger;
        _ipAddressService = ipAddressService;
        _accessPolicyService = accessPolicyService;
        _cache = memoryCache;
        _logger = logger;
        _domainService = domainService;
        _auditLogger = auditLogger;
        _eraService = eraService;
        _validationSettings = validationSettings.Value;
        _cacheDuration = TimeSpan.FromMinutes(_validationSettings.CacheDurationMinutes);
    }


    [HttpGet]
    public async Task<ActionResult> Validate([FromQuery] string? resource)
    {
        var validationResult = InitializeValidationResult(resource);

        try
        {
            //TODO: put all logic into policy.
            var policyResult = await _accessPolicyService.EvaluateIpAccessPolicy(validationResult.IpAddress);
            
            if (policyResult.IsApplied)
            {
                if (policyResult.Action == AccessAction.Allow)
                {
                    validationResult.Message = "Allowed by policy ";
                    _auditLogger.ExecuteWithAudit(AuditType.AllowedByPolicy, validationResult);
                    return Ok(new { Data = validationResult, auditCode = (int)AuditType.AllowedByPolicy});
                } else 
                if (policyResult.Action == AccessAction.Deny)
                {
                    return HandleValidationFailure(validationResult, "Denied by policy ", AuditType.BlockedByPolicy );
                }
            } else 
            {
                var validationAgregatedResult =
                        await GetComputerAggregatedValidationResult(validationResult.IpAddress);

                    validationResult = MapValidationResultToDto(validationResult, validationAgregatedResult.Data);
                    if (!validationAgregatedResult.ValidationResult.IsValid)
                    {
                        var auditType = validationAgregatedResult.ValidationResult.AuditType;
                        var message = validationAgregatedResult.ValidationResult.ErrorMessage;

                        return HandleValidationFailure(validationResult, message, auditType);
                    }
            }
            
            _auditLogger.ExecuteWithAudit(AuditType.Ok, validationResult);
            return Ok(new { Data = validationResult, auditCode = (int)AuditType.Ok });
        }
        catch (UnauthorizedAccessException)
        {
            return HandleUnauthorizedAccess(validationResult);
        }
        catch (Exception ex)
        {
            return HandleException(validationResult, ex);
        }
    }

    private ValidationResultDto InitializeValidationResult(string? resource)
    {
        return new ValidationResultDto
        {
            IpAddress = _ipAddressService.ExtractIPv4(_ipAddressService.GetRequestIP()),
            ResourceName = string.IsNullOrEmpty(resource) ? string.Empty : resource
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
            var successValidationResult = new ValidationResult<EraComputerInfo>();
            var successValidationResults = new List<ValidationResult<EraComputerInfo>> () {successValidationResult};
            validationAgregatedResult = new ValidationResultAgregated<EraComputerInfo>(cachedData, successValidationResult, successValidationResults  );
            return validationAgregatedResult;
        }
        
        var validationResults = await _validationService.ValidateWithEraAsync(ipAddress);
        
        
        var eraPriority = new List<AuditType> 
        { 
            AuditType.Ok, 
            AuditType.NotFoundEset, 
            AuditType.NotFoundDomain, 
            AuditType.NotValidEsetTimespan,
            AuditType.NotFound,
        };
        
        var okFirstVariant =  OrderValidationResultHelper.SelectValidationResultWithCustomPriority(validationResults, eraPriority);
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
    
    private ValidationResultDto MapValidationResultToDto(ValidationResultDto validationResult,
        EraComputerInfo computerData)
    {
        return new ValidationResultDto
        {
            IpAddress = computerData.IpAddress,
            Message = validationResult.Message,
            ResourceName = validationResult.ResourceName,
            Domain = computerData.Domain,
            Hostname = computerData.ComputerName,
            UserName = string.Empty
        };
    }

    private ActionResult HandleValidationFailure(
       ValidationResultDto validationResult,
       string errorMessage,
       AuditType auditType = AuditType.NotFound)
    {
        validationResult.Message = errorMessage ?? "Not found general error";
        
        _auditLogger.ExecuteWithAudit(auditType, validationResult);
        return Unauthorized(new
        {
            Data = validationResult,
            auditCode = (int)auditType,
            message = validationResult.Message,
        });
    }

    private ActionResult HandleUnauthorizedAccess(ValidationResultDto validationResult)
    {
        _auditLogger.ExecuteWithAudit(AuditType.NotFoundEset, validationResult);
        return Unauthorized(new
        {
            ipAddress = validationResult.IpAddress,
            auditCode = (int)AuditType.NotFoundEset
        });
    }

    private ActionResult HandleException(ValidationResultDto validationResult, Exception ex)
    {
        validationResult.Message = ex.Message;
        _auditLogger.ExecuteWithAudit(AuditType.NoAccessToDb, validationResult);
        return StatusCode(401, new
        {
            ipAddress = validationResult.IpAddress,
            auditCode = (int)AuditType.NoAccessToDb,
            error = ex.Message
        });
    }
}
