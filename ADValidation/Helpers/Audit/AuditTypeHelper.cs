using System.Collections;
using System.Runtime.CompilerServices;
using ADValidation.DTOs.Audit;
using ADValidation.Enums;

namespace ADValidation.Helpers.Audit;

public class AuditTypeHelper
{
    private static readonly Dictionary<AuditType, string> _auditRecords = new Dictionary<AuditType, string>()
    {
        { AuditType.Ok, "Дозволено" },
        { AuditType.NotFound, "Заблоковано: Не знайдено. Занальна помилка" },
        { AuditType.NotFoundDomain, "Заблоковано: Запис відсутній в домені" },
        { AuditType.NotFoundEset, "Заблоковано: Запис відсутній в антивірусі" },
        { AuditType.NotValidEsetTimespan, "Заблоковано: Машина була в мережі більше ніж 5 хв тому..."},
        { AuditType.NoAccessToDb, "Дозволено: Відсутній зв'язов з БД"},
        { AuditType.AllowedByPolicy, "Дозволено: політикою"},
        { AuditType.BlockedByPolicy, "Заблоковано: політикою"},
        { AuditType.AllowedAfterRetry, "Дозволено: після декількох спроб..."},
        { AuditType.AllowedByCache, "Дозволено: Запис існує в \"кеші\""},
    };

    private static readonly IEnumerable<AuditType> _auditRecordsAllow = new List<AuditType>()
    {
        AuditType.Ok,
        AuditType.AllowedByPolicy,
        AuditType.AllowedAfterRetry,
        AuditType.AllowedByCache,
        
        AuditType.NoAccessToDb
    };
    
    private static readonly IEnumerable<AuditType> _auditRecordsOk = new List<AuditType>()
    {
        AuditType.Ok,
        AuditType.AllowedByPolicy,
        AuditType.AllowedAfterRetry,
        AuditType.AllowedByCache,
    };
    
    private static readonly IEnumerable<AuditType> _auditRecordsAlert = new List<AuditType>()
    {
        AuditType.NoAccessToDb
    };

    public static string GetAuditTypeString(AuditType auditType)
    {
        return _auditRecords.TryGetValue(auditType, out string value) ? value : auditType.ToString();
    }

    public static bool GetAuditTypeAllowed(AuditType auditType)
    {
        return _auditRecordsAllow.Contains(auditType);
    }
    
    public static IEnumerable<AuditTypeInfo> GetAllAuditTypeInfos()
    {
        return Enum.GetValues(typeof(AuditType))
            .Cast<AuditType>()
            .Select(t => new AuditTypeInfo { Type = t });
    }

    public static AuditTypeStatus GetAuditTypeStatus(AuditType auditType)
    {
        if (_auditRecordsOk.Contains(auditType))
        {
            return AuditTypeStatus.OK;
        }
        
        if  (_auditRecordsAlert.Contains(auditType)) {
            return AuditTypeStatus.Alert;
        } 
        
        return AuditTypeStatus.Danger;
    }
}