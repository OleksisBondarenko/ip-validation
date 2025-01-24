namespace ADValidation.Models.ERA;

public class NetworkInfo
{
     public string Address { get; set; }
     public string Mac { get; set; }
     public DateTime Occurred { get; set; }
     public byte[] SourceUuid { get; set; }
}