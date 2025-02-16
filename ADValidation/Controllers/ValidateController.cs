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
    private readonly LDAPSettings _ldapSettings;
    private readonly AuditLoggerService _auditLogger;
    
    public ValidateController(DomainService domainService, AuditService auditService, AuditLoggerService auditLogger,
        IPAddressService ipAddressService, EraValidationService eraValidationService,
        IOptions<LDAPSettings> ldapSettings)
    {
        _domainService = domainService;
        _ipAddressService = ipAddressService;
        _ldapSettings = ldapSettings.Value;
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
        ValidationFailResult validationFailResult = new ValidationFailResult();
        
        string userIp = _ipAddressService.GetRequestIP();
        validationResult.IpAddress = _ipAddressService.ExtractIPv4(userIp);

        if (string.IsNullOrEmpty(validationResult.IpAddress))
        {
            validationFailResult.Message = "Please provide a valid IP address.";
            return Unauthorized(validationFailResult);
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
        
        foreach (var domain in _ldapSettings.Domains)
        {
            if (_domainService.IsHostnameInActiveDirectory(domain, validationResult.Hostname))
            {
                validationResult.Domain = domain.DomainName;
                validationResult.UserName = _domainService.GetUsernameFromHostname(domain, validationResult.Hostname);
            }
        }

        if (string.IsNullOrEmpty(validationResult.Domain))
        {
            _auditLogger.ExecuteWithAuditAsync(AuditType.NotFoundDomain, validationResult);
            validationFailResult.Message = "Hostname not found in any configured Active Directory domain.";
            return Unauthorized(validationFailResult);
        }

        _auditLogger.ExecuteWithAuditAsync(AuditType.AuthorizedAccess, validationResult);
        return Ok(validationResult);
    }
}