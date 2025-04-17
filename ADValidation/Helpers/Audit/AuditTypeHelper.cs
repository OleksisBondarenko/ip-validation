using ADValidation.Enums;

namespace ADValidation.Helpers.Audit;

public class AuditTypeHelper
{
    private static readonly Dictionary<AuditType, string> _auditRecords = new Dictionary<AuditType, string>()
    {
        { AuditType.Ok, "OK" },
        { AuditType.NotFound, "Не знайдено. Занальна помилка" },
        { AuditType.NotFoundDomain, "Запис відсутній в домені" },
        { AuditType.NotFoundEset, "Запис відсутній в антивірусі" },
        { AuditType.NotValidEsetTimespan, "Машина була в мережі більше ніж 5 хв тому..."},
        { AuditType.NoAccessToDb, "Відсутній зв'язов з БД"},
    };

    public static string GetAuditTypeString(AuditType auditType)
    {
        return _auditRecords.TryGetValue(auditType, out string value) ? value : auditType.ToString();
    }

}