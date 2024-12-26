using System.DirectoryServices.Protocols;
using ADValidation.Models;
using Microsoft.Extensions.Options;

namespace ADValidation.Services;

public class DomainService
{
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
                $"(&(objectClass=computer)(ipHostNumber=*))",
                // $"&(objectClass=computer)(name=test)",
                // (&(objectClass=user)(|(sn=Smith)(givenName=John)))
                SearchScope.Subtree,
                "cn" // Attributes to retrieve
            );

            var searchResponse = (SearchResponse)ldapConnection.SendRequest(searchRequest);

            var foundEntries = searchResponse.Entries;
            
            return searchResponse.Entries.Count > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error querying domain {domain.DomainName}: {ex.Message}");
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
            
            // Search request for DNS record in AD DS
            var searchRequest = new SearchRequest(
                domain.BaseDN,
                // $"(dnsHostName={hostname})", // Use the resolved hostname for DNS search
                    $"(&(objectClass=computer)(dNSHostName={hostname}))",
                SearchScope.Subtree,
                "cn" // Attributes to retrieve
            );

            var searchResponse = (SearchResponse)ldapConnection.SendRequest(searchRequest);
            var res = searchResponse.Entries;
            
            return searchResponse.Entries.Count > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error querying domain {domain.DomainName}: {ex.Message}");
            return false;
        }
    }
    
}

