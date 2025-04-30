using System.Reflection;
using ADValidation.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace ADValidation.Helpers.Ip;

public class  WhiteListIpConfigReader
{
    public string IpWhiteListRelativePath { get; set; }

    public WhiteListIpConfigReader(string ipWhiteListRelativePath)
    {
        IpWhiteListRelativePath = ipWhiteListRelativePath;
    }
    
    public string[] WhiteListIPs()
    {
        string configPath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), 
            IpWhiteListRelativePath
        );
    
        if (!File.Exists(configPath))
            return Array.Empty<string>(); // or throw an exception, depending on your needs

        return File.ReadAllLines(configPath);
    }

}