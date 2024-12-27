using System.DirectoryServices;
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

    public ValidateController(DomainService domainService, IPAddressService ipAddressService, IOptions<LDAPSettings> ldapSettings)
    {
        _domainService = domainService;
        _ipAddressService = ipAddressService;
        _ldapSettings = ldapSettings.Value;
    }

    [HttpGet]
    public IActionResult Validate()
    {
        string userIp = _ipAddressService.GetRequestIP();

        if (string.IsNullOrEmpty(userIp))
        {
            return BadRequest("IP address is missing.");
        }

        string resolvedHostname = DnsUtils.GetHostnameFromIp(userIp);
        resolvedHostname = "server2019.ad1.org";
        
        if (string.IsNullOrEmpty(resolvedHostname))
        {
            return BadRequest("IP address is not valid or hostname could not be resolved.");
        }

        foreach (var domain in _ldapSettings.Domains)
        {
            if (_domainService.IsHostnameInActiveDirectory(domain, resolvedHostname))
            {
                string username = _domainService.GetUsernameFromHostname(domain, resolvedHostname);
                
                return Ok(new { Username = username });
            }
        }

        return NotFound("Hostname not found in any configured Active Directory domain.");
    }

}
