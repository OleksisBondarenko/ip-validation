using ADValidation.Models.Audit;
using ADValidation.Services;
using System.Text.Json;
using System.Threading.Tasks;
using ADValidation.Enums;
using ADValidation.Helpers.Validators;
using ADValidation.Models;

namespace ADValidation.Decorators;

public class AuditLoggerService
{
    private readonly AuditService _auditService;

    public AuditLoggerService(AuditService auditService)
    {
        _auditService = auditService;
    }

    public void ExecuteWithAudit(GeneralValidationResult<ComputerInfo> validationResult)

    {
        var computerInfo = validationResult.Data;
        
        var auditData = new AuditData()
        {
            UserName = computerInfo.UserName,
            IpAddress = computerInfo.IpAddress,
            Hostname = computerInfo.Hostname,
            Domain = computerInfo.Domain,  
        };
        
        var auditRecord = new AuditRecord()
        {
            AuditType = validationResult.AuditType,
            ResourceName = computerInfo.ResourceName,
            AuditData = auditData,
        };

        _auditService.CreateAsync(auditRecord).Wait();
    }
}