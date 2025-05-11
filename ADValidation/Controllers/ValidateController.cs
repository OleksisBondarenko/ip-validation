using System.ComponentModel.DataAnnotations;
using ADValidation.Decorators;
using ADValidation.Enums;
using ADValidation.Helpers.OrderHelper;
using ADValidation.Models;
using ADValidation.Models.Api;
using ADValidation.Models.ERA;
using ADValidation.Services;
using ADValidation.Services.Validation;
using ADValidation.Validators;
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

    public ValidateController(
        IPAddressService ipAddressService,
        AuditLoggerService auditLogger,
        ValidationService validationService,
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
            var  validationAgregatedResult =
                await GetComputerAggregatedValidationResult(validationResult.IpAddress);
            
            validationResult = MapValidationResultToDto(validationResult, validationAgregatedResult.Data);

            if (!validationAgregatedResult.ValidationResult.IsValid)
            {
                return HandleValidationFailure(validationResult, validationAgregatedResult);
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

        var validationResults = await _validationService.ValidateAsync(ipAddress);
        
        var priorityOrder = new List<AuditType> 
        { 
            AuditType.Ok, 
            AuditType.NotFoundEset, 
            AuditType.NotFoundDomain, 
            AuditType.NotValidEsetTimespan,
            AuditType.NotFound,
        };
        
        var mostLessSeriousResult =  OrderValidationResultHelper.SelectValidationResultWithCustomPriority(validationResults, priorityOrder);
            // validationResults.FirstOrDefault(data => data.IsValid) ??
            //                          validationResults.OrderByDescending(data => data.AuditType)
            //                              .FirstOrDefault(data => !data.IsValid);
            //
        
        var computerData = mostLessSeriousResult?.Data ?? new EraComputerInfo { IpAddress = ipAddress };

        if (mostLessSeriousResult?.IsValid == true)
        {
            _cache.Set(cacheSuccessCompKey, computerData, _cacheDuration);
        }

        return new ValidationResultAgregated<EraComputerInfo>(computerData, mostLessSeriousResult, validationResults);
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
       ValidationResultAgregated<EraComputerInfo> validationResultAgregated)
    {
        AuditType auditType = (validationResultAgregated?.ValidationResult?.AuditType ?? AuditType.NotFound);
        validationResult.Message =
            validationResultAgregated?.ValidationResult?.ErrorMessage ?? "Not found general error";
        
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
