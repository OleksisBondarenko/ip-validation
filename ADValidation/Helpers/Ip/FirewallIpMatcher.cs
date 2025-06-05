using System.Net;
using System.Runtime.CompilerServices;
using SQLitePCL;

namespace ADValidation.Helpers.Ip;

public static class FirewallIpMatcher
{
    /// <summary>
    /// Determines whether a given IP address falls within a specified CIDR (Classless Inter-Domain Routing) subnet.
    /// </summary>
    /// <param name="ip">The IP address to check.</param>
    /// <param name="cidr">
    /// A CIDR notation string representing the subnet, e.g., "192.168.1.0/24".
    /// The prefix length (after the slash) indicates how many bits are fixed for the network.
    /// </param>
    /// <returns>
    /// True if the <paramref name="ip"/> is within the subnet specified by <paramref name="cidr"/>; otherwise, false.
    /// </returns>
    /// <exception cref="FormatException">
    /// Thrown if the CIDR string is not in a valid format or contains invalid IP address or prefix length.
    /// </exception>
    public static bool IsIpInRule(string ipToCheck, string rule)
    {
        var ip = IPAddress.Parse(ipToCheck);

        if (rule.Contains("/"))
        {
            // CIDR format
            return IsInCidrRange(ip, rule);
        }
        else if (rule.Contains("-"))
        {
            // IP Range format
            string[] parts = rule.Split('-');
            if (parts.Length == 2)
                return IsInIpRange(ip, IPAddress.Parse(parts[0]), IPAddress.Parse(parts[1]));
        }
        else
        {
            // Exact match
            return ip.Equals(IPAddress.Parse(rule));
        }

        return false;
    }

    /// <summary>
    /// Checks whether a given IP address falls within a specified IP range.
    /// </summary>
    /// <param name="IsIpInRule">The IP address to check (e.g., "192.168.1.50").</param>
    /// <param name="startIp">The start IP of the allowed range (e.g., "192.168.1.0").</param>
    /// <param name="endIp">The end IP of the allowed range (e.g., "192.168.1.255").</param>
    /// <returns>
    /// True if <paramref name="IsIpInRule"/> falls within the range defined by <paramref name="startIp"/> and <paramref name="endIp"/>; otherwise, false.
    /// </returns>
    /// <exception cref="FormatException">
    /// Thrown if any of the input strings are not valid IP addresses.
    /// </exception>
    ///
    // public static bool IsIpInRule(string ipToCheck, string startIp, string endIp)
    // {
    //     var ip = IPAddress.Parse(ipToCheck);
    //     return IsInIpRange(ip, IPAddress.Parse(startIp), IPAddress.Parse(endIp));
    // }
    // /// <summary>
    // /// Checks whether the given IP address matches any rule in the provided array of filter rules.
    // /// Each rule can be an exact IP, a CIDR subnet, or an IP range (e.g., "192.168.1.1", "192.168.1.0/24", "192.168.1.10-192.168.1.20").
    // /// </summary>
    // /// <param name="ipToCheck">The IP address to validate.</param>
    // /// <param name="rules">An array of IP filter rules.</param>
    // /// <returns>True if the IP address matches any of the rules; otherwise, false.</returns>
    // /// <exception cref="FormatException">
    // /// Thrown if any rule in the list is malformed or contains an invalid IP or CIDR.
    // /// </exception>
    
    public static bool IsIpInRule(string ipToCheck, string[] rules)
    {
        foreach (var rule in rules)
        {
            if (IsIpInRule(ipToCheck, rule, false))
            {
                return true;
            }
        }
        
        return false;
    }
    
    private static bool IsIpInRule(string ipToCheck, string rule, bool throwException)
    {
        try
        {
            return IsIpInRule(ipToCheck, rule);
        }
        catch (FormatException ex)
        {
            if (throwException) 
                throw ex;
            
            return false;
        }
    }
    
    private static bool IsInIpRange(IPAddress ip, IPAddress startIp, IPAddress endIp)
    {
        byte[] ipBytes = ip.GetAddressBytes();
        byte[] startBytes = startIp.GetAddressBytes();
        byte[] endBytes = endIp.GetAddressBytes();

        if (ipBytes.Length != startBytes.Length || ipBytes.Length != endBytes.Length)
            return false;

        for (int i = 0; i < ipBytes.Length; i++)
        {
            if (ipBytes[i] < startBytes[i]) return false;
            if (ipBytes[i] > endBytes[i]) return false;
        }

        return true;
    }

    private static bool IsInCidrRange(IPAddress ip, string cidr)
    {
        string[] parts = cidr.Split('/');
        if (parts.Length != 2)
            return false;

        IPAddress network = IPAddress.Parse(parts[0]);
        int prefixLength = int.Parse(parts[1]);

        byte[] ipBytes = ip.GetAddressBytes();
        byte[] networkBytes = network.GetAddressBytes();

        if (ipBytes.Length != networkBytes.Length)
            return false;

        int fullBytes = prefixLength / 8;
        int remainingBits = prefixLength % 8;

        for (int i = 0; i < fullBytes; i++)
        {
            if (ipBytes[i] != networkBytes[i])
                return false;
        }

        if (remainingBits > 0)
        {
            int mask = (byte)(~(0xFF >> remainingBits));
            if ((ipBytes[fullBytes] & mask) != (networkBytes[fullBytes] & mask))
                return false;
        }

        return true;
    }

}