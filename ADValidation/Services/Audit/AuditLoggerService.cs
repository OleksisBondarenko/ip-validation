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

    public void ExecuteWithAudit(AuditType type, ValidationResultDto validationResultDto)

    {
        var auditData = new AuditData()
        {
            UserName = validationResultDto.UserName,
            IpAddress = validationResultDto.IpAddress,
            Hostname = validationResultDto.Hostname,
            Domain = validationResultDto.Domain,  
        };
        
        var auditRecord = new AuditRecord()
        {
            AuditType = type,
            ResourceName = validationResultDto.ResourceName,
            AuditData = auditData,
        };

        _auditService.CreateAsync(auditRecord).Wait();
    }
}