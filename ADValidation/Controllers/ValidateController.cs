using ADValidation.Decorators;
using ADValidation.Enums;
using ADValidation.Models;
using ADValidation.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ADValidation.Controllers;

[ApiController]
[Route("[controller]")]
public class ValidateController : ControllerBase
{
    private readonly DomainService _domainService;
    private readonly EraValidationService _eraValidationService;
    private readonly IPAddressService _ipAddressService;
    private readonly AuditLoggerService _auditLogger;

    public ValidateController(DomainService domainService, AuditService auditService, AuditLoggerService auditLogger,
        IPAddressService ipAddressService, EraValidationService eraValidationService)
    {
        _domainService = domainService;
        _ipAddressService = ipAddressService;
        _eraValidationService = eraValidationService;
        _auditLogger = auditLogger;
    }

    [HttpGet]
    public async Task<ActionResult> Validate(
        [FromQuery] string? isUsername,
        [FromQuery] string? isEset,
        [FromQuery] string? isSafetica)
    {
        ValidationSuccessResult validationResult = new ValidationSuccessResult();

        string userIp = _ipAddressService.GetRequestIP();
        validationResult.IpAddress = _ipAddressService.ExtractIPv4(userIp);

        if (string.IsNullOrEmpty(validationResult.IpAddress))
        {
            validationResult.Message = "Please provide a valid IP address.";
            return Unauthorized(validationResult);
        }

        // TODO: update view of hostname. Here is only EXAMPLE!
        try
        {
            validationResult.Hostname = await _eraValidationService.GetHostByIp(validationResult.IpAddress);
        }
        catch (UnauthorizedAccessException e)
        {
            _auditLogger.ExecuteWithAuditAsync(AuditType.NotFoundAntivirus, validationResult);
            return Unauthorized(new { ipAddress = validationResult.IpAddress });
        }
        catch (Exception e)
        {
            validationResult.Message = e.Message;
            _auditLogger.ExecuteWithAuditAsync(AuditType.ApplicationCrashedPassAccess, validationResult);
            return Ok(new { ipAddress = validationResult.IpAddress });
        }

        try
        {
            validationResult.Domain = _domainService.GetDomainFromHostname(validationResult.Hostname);
        }
        catch (Exception e)
        {
            validationResult.Message = e.Message;
            _auditLogger.ExecuteWithAuditAsync(AuditType.ApplicationCrashedPassAccess, validationResult);
            return Ok(new { ipAddress = validationResult.IpAddress });
        }

        if (string.IsNullOrEmpty(validationResult.Domain))
        {
            _auditLogger.ExecuteWithAuditAsync(AuditType.NotFoundDomain, validationResult);
            validationResult.Message = "Hostname not found in any configured Active Directory domain.";
            return Unauthorized(validationResult);
        }

        _auditLogger.ExecuteWithAuditAsync(AuditType.AuthorizedAccess, validationResult);
        return Ok(validationResult);
    }
}