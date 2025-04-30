using System.ComponentModel.DataAnnotations;
using ADValidation.Decorators;
using ADValidation.Enums;
using ADValidation.Models;
using ADValidation.Models.Api;
using ADValidation.Models.ERA;
using ADValidation.Services;
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
    public ValidateController(
        IPAddressService ipAddressService,
        AuditLoggerService auditLogger ,   
        IMemoryCache memoryCache, 
        DomainService domainService,
        EraService eraService,
        ILogger<ValidateController> logger,
        IOptions<ValidationSettings> validationSettings
        )
    {
        
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
public async Task<ActionResult> Validate(
    [FromQuery] string? resource)
{
    // Initialize result objects
    var validationResult = new ValidationSuccessResult();
    
    // Get and validate IP address
    string userIp = _ipAddressService.GetRequestIP();
    validationResult.IpAddress = _ipAddressService.ExtractIPv4(userIp);
    validationResult.ResourceName = string.IsNullOrEmpty(resource) ? string.Empty : resource;

    if (_ipAddressService.IsWhiteListIp(validationResult.IpAddress))
    {
        validationResult.Message = "Bypass. ip in white-list ip addresses";
        _auditLogger.ExecuteWithAudit(AuditType.Ok, validationResult);
        return Ok(validationResult);
    }
    
    if (string.IsNullOrEmpty(validationResult.IpAddress))
    {
        validationResult.Message = "Please provide a valid IP address.";
        _auditLogger.ExecuteWithAudit(AuditType.NotFound, validationResult);
        return Unauthorized(validationResult);
    }

    try
    {
        // Try to get computer data with caching
       string cacheSuccessCompKey= $"comp_data_{validationResult.IpAddress}";
        if (!_cache.TryGetValue(cacheSuccessCompKey, out ComputerAggregatedData computerAggregatedData))
        {
            computerAggregatedData = await _eraService.GetComputerAggregatedData(validationResult.IpAddress);
            validationResult.Hostname = computerAggregatedData.ComputerName;
            
            // Check is ESET was active last 5 minutess
            int eraTimespanMilis = _validationSettings.EsetValidConnectionTimespan;
            DateTime modifiedConnectedTime =
                computerAggregatedData.ComputerConnected.Add(TimeSpan.FromMilliseconds(eraTimespanMilis));
            if (modifiedConnectedTime < DateTime.Now)
            {
                _auditLogger.ExecuteWithAudit(AuditType.NotValidEsetTimespan, validationResult);
                return Unauthorized(new
                {
                    Data = validationResult,
                    auditCode = (int)AuditType.NotValidEsetTimespan,
                    message =
                        $"Computer wasn`t online in ESET ERA more than {TimeSpan.FromMilliseconds(eraTimespanMilis)}"
                });
            }
            
            // Domain validation
            string domain = _domainService.GetDomainFromHostname(validationResult.Hostname);
            computerAggregatedData.Domain = domain;
            
            if (string.IsNullOrEmpty(domain))
            {
                _auditLogger.ExecuteWithAudit(AuditType.NotFoundDomain, validationResult);
                return Unauthorized(new {
                    auditCode = (int)AuditType.NotFoundDomain,
                    data = new ValidationSuccessResult()
                    {
                        Message = "Hostname not found in any configured Active Directory domain.",
                        Domain = domain,
                        ResourceName = validationResult.ResourceName,
                        Hostname = validationResult.Hostname,
                        IpAddress = validationResult.IpAddress,
                    },
                });
            }
            // Domain validation
            // try
            // {
            //     var domainCacheKey = $"domain_{validationResult.Hostname}";
            //     if (!_cache.TryGetValue(domainCacheKey, out string domain))
            //     {
            //         domain = _domainService.GetDomainFromHostname(validationResult.Hostname);
            //         _cache.Set(domainCacheKey, domain, _cacheDuration);
            //     }
            //     validationResult.Domain = domain;
            //
            //     if (string.IsNullOrEmpty(validationResult.Domain))
            //     {
            //         _auditLogger.ExecuteWithAudit(AuditType.NotFoundDomain, validationResult);
            //         return Unauthorized(new {
            //             ipAddress = validationResult.IpAddress,
            //             auditCode = (int)AuditType.NotFoundDomain,
            //             message = "Hostname not found in any configured Active Directory domain."
            //         });
            //     }
            // }
            // catch (Exception ex)
            // {
            //     validationResult.Message = ex.Message;
            //     _auditLogger.ExecuteWithAudit(AuditType.NoAccessToDb, validationResult);
            //     return StatusCode(401, new {
            //         ipAddress = validationResult.IpAddress,
            //         auditCode = (int)AuditType.NoAccessToDb,
            //         error = ex.Message
            //     });
            // }
        }
        
        _cache.Set(cacheSuccessCompKey, computerAggregatedData, _cacheDuration);
        validationResult.Domain = computerAggregatedData.Domain;
        validationResult.Hostname = computerAggregatedData.ComputerName;
    }
    catch (UnauthorizedAccessException)
    {
        _auditLogger.ExecuteWithAudit(AuditType.NotFoundEset, validationResult);
        return Unauthorized(new { 
            ipAddress = validationResult.IpAddress,
            auditCode = (int)AuditType.NotFoundEset
        });
    }
    catch (Exception ex)
    {
        validationResult.Message = ex.Message;
        _auditLogger.ExecuteWithAudit(AuditType.NoAccessToDb, validationResult);
        return StatusCode(401, new {
            ipAddress = validationResult.IpAddress,
            auditCode = (int)AuditType.NoAccessToDb,
            error = ex.Message
        });
    }

    // Successful validation
    _auditLogger.ExecuteWithAudit(AuditType.Ok, validationResult);
    return Ok(new {
        Data = validationResult,
        auditCode = (int)AuditType.Ok,
    });
}
}