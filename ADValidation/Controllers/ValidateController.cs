using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Net;
using ADValidation.Decorators;
using ADValidation.Enums;
using ADValidation.Models;
using ADValidation.Models.Audit;
using ADValidation.Models.ERA;
using ADValidation.Services;
using ADValidation.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace ADValidation.Middleware;

[ApiController]
[Route("[controller]")]
public class ValidateController : ControllerBase
{
    private readonly DomainService _domainService;
    private readonly EraValidationService _eraValidationService;
    private readonly IPAddressService _ipAddressService;
    private readonly LDAPSettings _ldapSettings;
    private readonly AuditLoggerService _auditLogger;
    private readonly AuditService _auditService;

    public ValidateController(DomainService domainService, AuditService auditService, AuditLoggerService auditLogger,
        IPAddressService ipAddressService, EraValidationService eraValidationService,
        IOptions<LDAPSettings> ldapSettings)
    {
        _domainService = domainService;
        _ipAddressService = ipAddressService;
        _ldapSettings = ldapSettings.Value;
        _eraValidationService = eraValidationService;
        _auditLogger = auditLogger;
        _auditService = auditService;
    }

    [HttpGet]
    public async Task<ActionResult> Validate([FromQuery] string? isUsername, [FromQuery] string? isEset,
        [FromQuery] string? isSafetica)
    {
        string userIp = _ipAddressService.GetRequestIP();
        string ipv4Address = _ipAddressService.ExtractIPv4(userIp);

        if (string.IsNullOrEmpty(ipv4Address))
        {
            return Unauthorized(new { message = "Please provide a valid IP address.", ipAddress = ipv4Address });
        }

        // TODO: update view of hostname. Here is only EXAMPLE!
        string hostname = String.Empty;

        try
        {
            hostname = await _eraValidationService.GetHostByIp(ipv4Address);
        }
        catch (UnauthorizedAccessException e)
        {
            _auditLogger.ExecuteWithAuditAsync(AuditType.NotFoundAntivirus, ipv4Address);
            return Unauthorized(new { ipAddress = ipv4Address });
        }

        string username = String.Empty;
        string domainName = String.Empty;
        foreach (var domain in _ldapSettings.Domains)
        {
            if (_domainService.IsHostnameInActiveDirectory(domain, hostname))
            {
                domainName = domain.DomainName;
                username = _domainService.GetUsernameFromHostname(domain, hostname);
            }
        }

        if (string.IsNullOrEmpty(domainName))
        {
            _auditLogger.ExecuteWithAuditAsync(AuditType.AuthorizedAccess, ipv4Address, hostname, domainName);
            return Unauthorized(new
            {
                message = "Hostname not found in any configured Active Directory domain.", ipAddress = ipv4Address,
                Hostname = hostname
            });
        }

        _auditLogger.ExecuteWithAuditAsync(AuditType.AuthorizedAccess, ipv4Address, hostname, domainName);
        return Ok(new { Username = username, IpAddress = ipv4Address, Hostname = hostname, Domain = domainName });
    }

    [HttpGet("records/")]
    public async Task<ActionResult> GetRecords()
    {
        var records = await _auditService.GetAllAsync(0, Int32.MaxValue);
        return Ok(records);
    }
}