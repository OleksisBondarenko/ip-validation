using System.ComponentModel;

namespace ADValidation.Models.ERA;

public class ComputerAggregatedData
{
    [DisplayName("computer_id")]
    public int ComputerId { get; set; }
    
    [DisplayName("computer_uuid")]
    public byte [] ComputerGUID { get; set; } 

    [DisplayName("computer_name")]
    public string ComputerName { get; set; } // ESET`s computer name

    [DisplayName("computer_connected")]
    public DateTime ComputerConnected { get; set; } // last connected time

    [DisplayName("ip_address")]
    public string IpAddress { get; set; } // IpV4address {0-255}.{0-255}.{0-255}.{0-255} or IpV6 extremely rare
    
    [DisplayName("domain")]
    public string Domain { get; set; } // domain

    public ComputerAggregatedData ()
    {
        this.ComputerId = 0;
        this.ComputerGUID = new byte[16];
        this.ComputerName = string.Empty;
        this.ComputerConnected = DateTime.MinValue;
        this.IpAddress = string.Empty;
    }

}