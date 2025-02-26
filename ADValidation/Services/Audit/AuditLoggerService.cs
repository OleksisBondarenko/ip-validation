using ADValidation.Models.Audit;
using ADValidation.Services;
using System.Text.Json;
using System.Threading.Tasks;
using ADValidation.Models;

namespace ADValidation.Decorators;

public class AuditLoggerService
{
    private readonly AuditService _auditService;

    public AuditLoggerService(AuditService auditService)
    {
        _auditService = auditService;
    }

    public void ExecuteWithAuditAsync(string actionName, ValidationSuccessResult validationResult)

    {
        var auditData = new AuditData()
        {
            UserName = validationResult.UserName,
            IpAddress = validationResult.IpAddress,
            Hostname = validationResult.Hostname,
            Domain = validationResult.Domain,           
            Message = validationResult.Message,

        };

        var auditRecord = new AuditRecord()
        {
            Name = actionName,
            AuditData = auditData,
        };

        _auditService.CreateAsync(auditRecord).Wait();
    }
}