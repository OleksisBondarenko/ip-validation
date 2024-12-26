namespace ADValidation.Utils;

using System;
using System.Net;

public class DnsUtils
{
    public static string GetHostnameFromIp(string ipAddress)
    {
        try
        {
            var hostEntry = Dns.GetHostEntry(ipAddress);
            return hostEntry.HostName;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error resolving IP address {ipAddress}: {ex.Message}");
            return null;
        }
    }
}
