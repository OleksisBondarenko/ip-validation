namespace ADValidation.Models;

public class ValidationSettings
{
    public List<string> CheckDomainBy {get; set;}
    public int EsetValidConnectionTimespan { get; set; }
    public string WhiteListConfigPath { get; set; }
    public int CacheDurationMinutes { get; set; }
}