namespace ADValidation.Models.Audit;

public class AuditData
{
    public Guid Id { get; set; }
    public string IpAddress { get; set; }
    public string Hostname { get; set; }
    public string UserName { get; set; }
    public string Domain { get; set; }
    public AuditRecord AuditRecord { get; set; }
    public Guid AuditRecordId { get; set; }
}