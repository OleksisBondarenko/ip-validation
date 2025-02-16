using ADValidation.Models.Audit;
using ADValidation.Services;
using System.Text.Json;
using System.Threading.Tasks;

namespace ADValidation.Decorators;

public class AuditLoggerService
{
    private readonly AuditService _auditService;

    public AuditLoggerService(AuditService auditService)
    {
        _auditService = auditService;
    }

    public void ExecuteWithAuditAsync(string actionName, string ipAddress = "", string hostname = "", string domain = "")

    {
        var auditData = new AuditData()
        {
            IpAddress = ipAddress,
            Hostname = hostname,
            Domain = domain,
            
        };

        var auditRecord = new AuditRecord()
        {
            Name = actionName,
            AuditData = auditData
        };

        _auditService.CreateAsync(auditRecord).Wait();
    }
}