namespace ADValidation.Models.Computer;

public class IComputerInfo
{
    public string Hostname { get; set; }
    public string Domain { get; set; }
    public string FQDN { get; set; }
    public string IPAddress { get; set; }
    public DateTime LastConnected { get; set; }
}