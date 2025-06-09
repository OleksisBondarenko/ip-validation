using System.Collections;
using System.Runtime.CompilerServices;
using ADValidation.DTOs.Audit;
using ADValidation.Enums;

namespace ADValidation.Helpers.Audit;

public class AuditTypeHelper
{
    private static readonly Dictionary<AuditType, string> _auditRecordsToStringForAdmin = new Dictionary<AuditType, string>()
    {
        { AuditType.Ok, "Дозволено" },
        { AuditType.NotFound, "Заблоковано: Не знайдено. Загальна помилка" },
        { AuditType.NotFoundDomain, "Заблоковано: Запис відсутній в домені" },
        { AuditType.NotFoundEset, "Заблоковано: Запис відсутній в антивірусі" },
        { AuditType.NotValidEsetTimespan, "Заблоковано: Машина була в мережі більше ніж 5 хв тому..."},
        { AuditType.NoAccessToDb, "Дозволено: Відсутній зв'язок з однією з БД"},
        { AuditType.AllowedByPolicy, "Дозволено: політикою"},
        { AuditType.BlockedByPolicy, "Заблоковано: політикою"},
        { AuditType.AllowedAfterRetry, "Дозволено: після декількох спроб..."},
        { AuditType.AllowedByCache, "Дозволено: Запис існує в \"кеші\""},
    };

    private static readonly Dictionary<AuditType, string> _auditRecordsToStringForUsers = new Dictionary<AuditType, string>()
    {
        { AuditType.Ok, "Дозволено" },
        { AuditType.NotFound, "Заблоковано: зверніться до адміністратора, номер помилки: " + (int)AuditType.NotFound },
        { AuditType.NotFoundDomain, "Заблоковано: не вдалось отримати дані про АП, номер помилки: " + (int)AuditType.NotFoundDomain },
        { AuditType.NotFoundEset, "Заблоковано: не вдалось отримати дані про АП, номер помилки: " + (int)AuditType.NotFoundEset },
        { AuditType.NotValidEsetTimespan, "Заблоковано: дані комп'ютера не збігаються, номер помилки: " + (int)AuditType.NotValidEsetTimespan }, 
        { AuditType.NoAccessToDb, "Дозволено, номер помилки: " + (int)AuditType.NoAccessToDb },
        { AuditType.AllowedByPolicy, "Дозволено, номер помилки: " + (int)AuditType.AllowedByPolicy },
        { AuditType.BlockedByPolicy, "Заблоковано, заблоковано політикою: номер помилки: " + (int)AuditType.BlockedByPolicy },
        { AuditType.AllowedAfterRetry, "Дозволено, після декількох спроб...: номер помилки: " + (int)AuditType.AllowedAfterRetry },
        { AuditType.AllowedByCache, "Дозволено, Запис існує в \"кеші\": номер помилки: " + (int)AuditType.AllowedByCache },
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

    public static string GetAuditTypeStringForAdmin(AuditType auditType)
    {
        return _auditRecordsToStringForAdmin.TryGetValue(auditType, out string value) ? value : auditType.ToString();
    }
    
    public static string GetAuditTypeStringForUser(AuditType auditType)
    {
        return _auditRecordsToStringForUsers.TryGetValue(auditType, out string value) ? value : auditType.ToString();
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