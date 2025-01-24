using ADValidation.Models;
using ADValidation.Services;
using ADValidation.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ADValidation.Services
{
    public class ValidationService
    {
        private readonly DomainService _domainService;
        private readonly IPAddressService _ipAddressService;
        private readonly LDAPSettings _ldapSettings;
        private readonly ILogger<ValidationService> _logger;

        public ValidationService(
            DomainService domainService, 
            IPAddressService ipAddressService, 
            IOptions<LDAPSettings> ldapSettings, 
            ILogger<ValidationService> logger)
        {
            _domainService = domainService;
            _ipAddressService = ipAddressService;
            _ldapSettings = ldapSettings.Value;
            _logger = logger;
        }

        public bool IsDNSPartOfDomain(string hostname)
        {
            if (string.IsNullOrEmpty(hostname))
            {
                _logger.LogWarning($"hostname is null {hostname}");
                throw new ArgumentNullException(nameof(hostname));
            }
            
            foreach (var domain in _ldapSettings.Domains)
            {
                if (_domainService.IsHostnameInActiveDirectory(domain, hostname))
                {
                    // Return Ok() or you could redirect here, or some other action
                    _logger.LogInformation($"Host {hostname} is part of domain {domain}");
                    return true; // For example redirecting to youtube
                }
            }

            return false; // Validation failed
        }
    }
}
