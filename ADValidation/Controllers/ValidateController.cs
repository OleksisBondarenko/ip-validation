using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Net;
using ADValidation.Models;
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

    public ValidateController(DomainService domainService, IPAddressService ipAddressService, EraValidationService eraValidationService, IOptions<LDAPSettings> ldapSettings)
    {
        _domainService = domainService;
        _ipAddressService = ipAddressService;
        _ldapSettings = ldapSettings.Value;
        _eraValidationService = eraValidationService;
    }

    [HttpGet]
    public async Task<ActionResult> Validate([FromQuery] string? isUsername, [FromQuery] string? isEset, [FromQuery] string? isSafetica)
    {
        string userIp = _ipAddressService.GetRequestIP();
        string ipv4Address = _ipAddressService.ExtractIPv4(userIp);
        
        if (string.IsNullOrEmpty(ipv4Address))
        {
            return BadRequest(new { message = "Please provide a valid IP address." , ipAddress = ipv4Address }); 
        }
    
        // TODO: update view of hostname. Here is only EXAMPLE!
        string hostname = String.Empty;
        
        // if (!string.IsNullOrEmpty(isEset))
        // {
            hostname = await _eraValidationService.GetHostByIp(ipv4Address);
            //hostname = "DESKTOP-85HF40J.ad1.org";
           
            if (string.IsNullOrEmpty(hostname))
            {
                return BadRequest(new { ipAddress = ipv4Address });
            }
            
            
        // }
        // string resolvedHostname = DnsUtils.GetHostnameFromIp(userIp);
        //
        // if (string.IsNullOrEmpty(resolvedHostname))
        
        // {
        //     return BadRequest(new {message = "Please provide a valid IP address.", ipAddress = userIp });
        // }

        // if (string.IsNullOrEmpty(isDomain))
        // {
        
            foreach (var domain in _ldapSettings.Domains)
            {
                if (_domainService.IsHostnameInActiveDirectory(domain, hostname))
                {
                    string username = _domainService.GetUsernameFromHostname(domain, hostname);
                    return Ok(new { Username = username, IpAddress = ipv4Address, Hostname = hostname, Domain = domain.DomainName });
                } 
            }
            
            return BadRequest(new { message = "Hostname not found in any configured Active Directory domain.", ipAddress = ipv4Address });

        // }

    }
    
}
