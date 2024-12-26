using System.DirectoryServices.Protocols;
using ADValidation.Models;
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
    private readonly IPAddressService _ipAddressService;
    private readonly LDAPSettings _ldapSettings;
    
    
    public ValidateController(DomainService domainService, IPAddressService ipAddressService, IOptions<LDAPSettings> ldapSettings )
    {
        _domainService = domainService;
        _ipAddressService = ipAddressService;
        _ldapSettings = ldapSettings.Value;
    }

    [HttpGet]
    public IActionResult Validate()
    {
        string userIp = _ipAddressService.GetRequestIP();
        
        string resolvedHostname = DnsUtils.GetHostnameFromIp(userIp);
        
        resolvedHostname = "test";
        
        if (resolvedHostname == null)
        {
            return BadRequest("IP address is not valid");
        }

        if (string.IsNullOrEmpty(userIp))
        {
            return BadRequest("IP address is missing.");
        }

        foreach (var domain in _ldapSettings.Domains)
        {
            if (_domainService.IsHostnameInActiveDirectory(domain, resolvedHostname))
            {
                // return Ok(); // Validation success
                return Redirect("https://youtube.com");
            }
        }

        return NotFound(); // Validation failed
    }
}