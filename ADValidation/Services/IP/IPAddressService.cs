using System.Text.RegularExpressions;
using Microsoft.Extensions.Primitives;

namespace ADValidation.Services;

public class IPAddressService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IPAddressService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetRequestIP(bool tryUseXForwardHeader = true)
    {
        string ip = string.Empty;

        // todo support new "Forwarded" header (2014) https://en.wikipedia.org/wiki/X-Forwarded-For

        // X-Forwarded-For (csv list):  Using the First entry in the list seems to work
        // for 99% of cases however it has been suggested that a better (although tedious)
        // approach might be to read each IP from right to left and use the first public IP.
        // http://stackoverflow.com/a/43554000/538763
        //
        if (tryUseXForwardHeader)
            ip = SplitCsv(GetHeaderValueAs<string>("X-Forwarded-For", _httpContextAccessor)).FirstOrDefault();

        // RemoteIpAddress is always null in DNX RC1 Update1 (bug).
        if (string.IsNullOrWhiteSpace(ip) && _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress != null)
            ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

        if (string.IsNullOrWhiteSpace(ip))
            ip = GetHeaderValueAs<string>("REMOTE_ADDR", _httpContextAccessor);

        // _httpContextAccessor.HttpContext?.Request?.Host this is the local host.

        if (string.IsNullOrWhiteSpace(ip))
            throw new Exception("Unable to determine caller's IP.");

        return ip;
    }

    
    public string ExtractIPv4(string input)
    {
        // Define the regex pattern for an IPv4 address
        string ipv4Pattern = @"\b(?:[0-9]{1,3}\.){3}[0-9]{1,3}\b";

        // Match the pattern
        Match match = Regex.Match(input, ipv4Pattern);

        // Return the matched IPv4 address or null if not found
        return match.Success ? match.Value : string.Empty;
    }
    
    private T GetHeaderValueAs<T>(string headerName, IHttpContextAccessor _httpContextAccessor)
    {
        StringValues values;

        if (_httpContextAccessor.HttpContext?.Request?.Headers?.TryGetValue(headerName, out values) ?? false)
        {
            string rawValues = values.ToString(); // writes out as Csv when there are multiple.

            if (!String.IsNullOrWhiteSpace(rawValues))
                return (T)Convert.ChangeType(values.ToString(), typeof(T));
        }

        return default(T);
    }

    private List<string> SplitCsv(string csvList, bool nullOrWhitespaceInputReturnsNull = false)
    {
        if (string.IsNullOrWhiteSpace(csvList))
            return nullOrWhitespaceInputReturnsNull ? null : new List<string>();

        return csvList
            .TrimEnd(',')
            .Split(',')
            .AsEnumerable<string>()
            .Select(s => s.Trim())
            .ToList();
    }
}