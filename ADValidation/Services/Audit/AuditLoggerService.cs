using ADValidation.Models.Audit;
using ADValidation.Services;
using System.Text.Json;
using System.Threading.Tasks;
using ADValidation.Enums;
using ADValidation.Models;

namespace ADValidation.Decorators;

public class AuditLoggerService
{
    private readonly AuditService _auditService;

    public AuditLoggerService(AuditService auditService)
    {
        _auditService = auditService;
    }

    public void ExecuteWithAudit(AuditType type, ValidationSuccessResult validationResult)

    {
        var auditData = new AuditData()
        {
            UserName = validationResult.UserName,
            IpAddress = validationResult.IpAddress,
            Hostname = validationResult.Hostname,
            Domain = validationResult.Domain,  
        };
        
        var auditRecord = new AuditRecord()
        {
            AuditType = type,
            ResourceName = validationResult.ResourceName,
            AuditData = auditData,
        };

        _auditService.CreateAsync(auditRecord).Wait();
    }
}