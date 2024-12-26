namespace ADValidation.Models;

public class LDAPDomain
{
    public string DomainName { get; set; }
    public string DomainController { get; set; }
    public string BaseDN { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}