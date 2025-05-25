using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
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
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ADValidation.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ValidateController : ControllerBase
{
    private readonly AuditLoggerService _auditLogger;
    private readonly IPAddressService _ipAddressService;
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
        _logger = logger;
        _domainService = domainService;
        _auditLogger = auditLogger;
        _eraService = eraService;
        _validationSettings = validationSettings.Value;
        _cacheDuration = TimeSpan.FromMinutes(_validationSettings.CacheDurationMinutes);
    }


    [HttpGet]
    public async Task<ActionResult> Validate([FromQuery] string? resource, int retries = 9, int delayBetweenRetries = 3000 )
    {
        var validationResult = await _validationService.GetComputerValidationResultAsync(resource);
        
        int retriesLeft = retries;
        do
        {
            if (validationResult.IsValid && retries != retriesLeft)
            {
                validationResult.AuditType = AuditType.AllowedAfterRetry;
                _auditLogger.ExecuteWithAudit(validationResult);
                return Ok(validationResult);
            }
            
            if (validationResult.IsValid)
            {                
                _auditLogger.ExecuteWithAudit(validationResult);
                return Ok(validationResult);
            }
            validationResult = await _validationService.GetComputerValidationResultAsync(resource);

            await Task.Delay(delayBetweenRetries);
            retriesLeft--;
        } while (retriesLeft > 0);
        
        _auditLogger.ExecuteWithAudit(validationResult);
        return Unauthorized(validationResult);
        
        // var validationResult = new ValidationResultDto();
        //
        // var validationAgregatedResult =
        //     await GetComputerAggregatedValidationResult(validationResult.IpAddress);
        //
        // validationResult = MapValidationResultToDto(validationResult, validationAgregatedResult.Data);
        // if (!validationAgregatedResult.ComputerValidationResult.IsValid)
        // {
        //     var auditType = validationAgregatedResult.ComputerValidationResult.AuditType;
        //     var message = validationAgregatedResult.ComputerValidationResult.ErrorMessage;
        //
        //     return HandleValidationFailure(validationResult, message, auditType);
        // }
        // try {
        //      
        //      _auditLogger.ExecuteWithAudit(AuditType.Ok, validationResult);
        //      return Ok(new { Data = validationResult, auditCode = (int)AuditType.Ok });
        //  }
        //  catch (UnauthorizedAccessException)
        //  {
        //      return HandleUnauthorizedAccess(validationResult);
        //  }
        //  catch (Exception ex)
        //  {
        //      return HandleException(validationResult, ex);
        //  }
    }

    

    
   

    // private ActionResult HandleValidationFailure(
    //    ValidationResultDto validationResult,
    //    string errorMessage,
    //    AuditType auditType = AuditType.NotFound)
    // {
    //     validationResult.Message = errorMessage ?? "Not found general error";
    //     
    //     _auditLogger.ExecuteWithAudit(auditType, validationResult);
    //     return Unauthorized(new
    //     {
    //         Data = validationResult,
    //         auditCode = (int)auditType,
    //         message = validationResult.Message,
    //     });
    // }

    // private ActionResult HandleUnauthorizedAccess(ValidationResultDto validationResult)
    // {
    //     _auditLogger.ExecuteWithAudit(AuditType.NotFoundEset, validationResult);
    //     return Unauthorized(new
    //     {
    //         ipAddress = validationResult.IpAddress,
    //         auditCode = (int)AuditType.NotFoundEset
    //     });
    // }

    // private ActionResult HandleException(ValidationResultDto validationResult, Exception ex)
    // {
    //     validationResult.Message = ex.Message;
    //     _auditLogger.ExecuteWithAudit(AuditType.NoAccessToDb, validationResult);
    //     return StatusCode(401, new
    //     {
    //         ipAddress = validationResult.IpAddress,
    //         auditCode = (int)AuditType.NoAccessToDb,
    //         error = ex.Message
    //     });
    // }
}
