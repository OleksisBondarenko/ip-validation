namespace ADValidation.Enums;

public enum AuditType
{
    // public const string ApplicationCrashedPassAccess = "ApplicationCrashedPassAccess";
    // public const string UnautorizedAccess = "UnautorizedAccess";
    // public const string AuthorizedAccess = "AuthorizedAccess";
    // public const string NotFoundAntivirus = "NotFoundAntivirus";
    // public const string NotFoundDomain = "NotFoundDomain";
    
    Ok = 0,
    NotFound = 1,
    NotFoundDomain = 2,
    NotFoundEset = 3,
    NotValidEsetTimespan = 4,
    NoAccessToDb = 10
}