using System.DirectoryServices.Protocols;
using ADValidation.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ADValidation.Services
{
    public class DomainService
    {
        private readonly ILogger<DomainService> _logger;

        // Constructor with logger dependency injection
        public DomainService(ILogger<DomainService> logger)
        {
            _logger = logger;
        }

        public string GetUsernameFromIp(LDAPDomain domain, string ipAddress)
        {
            try
            {
                var ldapConnection = new LdapConnection(domain.DomainController);
                ldapConnection.Credential = new System.Net.NetworkCredential(
                    domain.Username,
                    domain.Password,
                    domain.DomainName
                );
                ldapConnection.AuthType = AuthType.Negotiate;

                // Search request for the IP address
                var searchRequest = new SearchRequest(
                    domain.BaseDN,
                    $"(&(objectClass=computer)(ipHostNumber={ipAddress}))",
                    SearchScope.Subtree,
                    "cn", "managedBy" // Attributes to retrieve
                );

                var searchResponse = (SearchResponse)ldapConnection.SendRequest(searchRequest);

                if (searchResponse.Entries.Count > 0)
                {
                    var managedBy = searchResponse.Entries[0].Attributes["managedBy"]?.GetValues(typeof(string))?.FirstOrDefault()?.ToString();

                    if (!string.IsNullOrEmpty(managedBy))
                    {
                        // Retrieve the username associated with the managedBy attribute
                        return GetUsernameFromDistinguishedName(domain, managedBy);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying domain {DomainName} for IP {IpAddress}", domain.DomainName, ipAddress);
            }

            return null;
        }

        public string GetUsernameFromHostname(LDAPDomain domain, string hostname)
        {
            try
            {
                var ldapConnection = new LdapConnection(domain.DomainController);
                ldapConnection.Credential = new System.Net.NetworkCredential(
                    domain.Username,
                    domain.Password,
                    domain.DomainName
                );
                ldapConnection.AuthType = AuthType.Negotiate;

                // Search request for the hostname
                var searchRequest = new SearchRequest(
                    domain.BaseDN,
                    $"(dNSHostName={hostname})",
                    SearchScope.Subtree,
                    "cn", "managedBy" // Attributes to retrieve
                );

                var searchResponse = (SearchResponse)ldapConnection.SendRequest(searchRequest);

                if (searchResponse.Entries.Count > 0)
                {
                    var managedBy = searchResponse.Entries[0].Attributes["managedBy"]?.GetValues(typeof(string))?.FirstOrDefault()?.ToString();

                    if (!string.IsNullOrEmpty(managedBy))
                    {
                        // Retrieve the username associated with the managedBy attribute
                        return GetUsernameFromDistinguishedName(domain, managedBy);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying domain {DomainName} for hostname {Hostname}", domain.DomainName, hostname);
            }

            return null;
        }

        private string GetUsernameFromDistinguishedName(LDAPDomain domain, string distinguishedName)
        {
            try
            {
                var ldapConnection = new LdapConnection(domain.DomainController);
                ldapConnection.Credential = new System.Net.NetworkCredential(
                    domain.Username,
                    domain.Password,
                    domain.DomainName
                );
                ldapConnection.AuthType = AuthType.Negotiate;

                // Search request for the distinguished name
                var searchRequest = new SearchRequest(
                    domain.BaseDN,
                    $"(distinguishedName={distinguishedName})",
                    SearchScope.Subtree,
                    "sAMAccountName" // Attribute to retrieve
                );

                var searchResponse = (SearchResponse)ldapConnection.SendRequest(searchRequest);

                if (searchResponse.Entries.Count > 0)
                {
                    return searchResponse.Entries[0].Attributes["sAMAccountName"]?.GetValues(typeof(string))?.FirstOrDefault()?.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving username from distinguished name {DistinguishedName}", distinguishedName);
            }

            return null;
        }
        public bool IsIpInActiveDirectory(LDAPDomain domain, string ipAddress)
        {
            try
            {
                var ldapConnection = new LdapConnection(domain.DomainController);
                ldapConnection.Credential = new System.Net.NetworkCredential(
                    domain.Username,
                    domain.Password,
                    domain.DomainName
                );
                ldapConnection.AuthType = AuthType.Negotiate;

                // Search request for IP address
                var searchRequest = new SearchRequest(
                    domain.BaseDN,
                    $"(&(objectClass=computer)(ipHostNumber={ipAddress}))",
                    SearchScope.Subtree,
                    "cn" // Attributes to retrieve
                );

                var searchResponse = (SearchResponse)ldapConnection.SendRequest(searchRequest);

                return searchResponse.Entries.Count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying domain {DomainName} for IP {IpAddress}", domain.DomainName, ipAddress);
                return false;
            }
        }

        public bool IsHostnameInActiveDirectory(LDAPDomain domain, string hostname)
        {
            try
            {
                var ldapConnection = new LdapConnection(domain.DomainController);
                ldapConnection.Credential = new System.Net.NetworkCredential(
                    domain.Username,
                    domain.Password,
                    domain.DomainName
                );
                ldapConnection.AuthType = AuthType.Negotiate;

                // Search for the computer object itself
                var searchRequest = new SearchRequest(
                    domain.BaseDN,
                    $"(&(objectClass=computer)(dNSHostName={hostname}))",
                    SearchScope.Subtree,
                    "cn" // Attributes to retrieve
                );

                var searchResponse = (SearchResponse)ldapConnection.SendRequest(searchRequest);

                // If the computer object exists, return true
                if (searchResponse.Entries.Count > 0)
                {
                    return true;
                }

                // If no computer object is found, check DNS records
                var dnsSearchRequest = new SearchRequest(
                    domain.BaseDN,
                    $"(&(objectClass=dnsNode)(name={hostname}))",
                    SearchScope.Subtree,
                    "cn"
                );

                var dnsSearchResponse = (SearchResponse)ldapConnection.SendRequest(dnsSearchRequest);

                return dnsSearchResponse.Entries.Count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying domain {DomainName} for hostname {Hostname}", domain.DomainName, hostname);
                return false;
            }
        }

    }
}
