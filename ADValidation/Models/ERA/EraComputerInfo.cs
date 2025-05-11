using System.ComponentModel;
using ADValidation.Helpers.LDAP;
using ADValidation.Services;

namespace ADValidation.Models.ERA;

public class EraComputerInfo
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
    public string Domain
    {
        get
        {
            return _domain;
        }
        set
        {
          _domain = value;  
        } 
    } // domain

    private string _domain;
    
    public EraComputerInfo ()
    {
        this.ComputerId = 0;
        this.ComputerGUID = new byte[16];
        this.ComputerName = string.Empty;
        this.ComputerConnected = DateTime.MinValue;
        this.IpAddress = string.Empty;
        this.Domain = string.Empty;
    }

    public override string ToString()
    {
        return this.ComputerName;
    }
}