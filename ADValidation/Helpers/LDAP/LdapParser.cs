namespace ADValidation.Helpers.LDAP;

public class LdapParser
{
    /// <summary>
    /// Parses baseDN into domain suffix.
    /// For example: "DC=ad1,DC=org"
    /// Parses into: ad1.org
    /// </summary>
    /// <param name="baseDn"></param>
    /// <returns>baseDn suffix</returns>
    public static string ParseLdapDomain(string baseDn)
    {
        return string.Join(".", 
            baseDn.Split(',')
                .Select(part => part.Split('=')[1].Trim())
                .Where(part => !string.IsNullOrEmpty(part)));
    }
}