using ADValidation.Helpers.Validators;
using ADValidation.Models;
using ADValidation.Models.ERA;
using ADValidation.Services.Validation;
using Microsoft.Extensions.Options;

namespace ADValidation.Services;

public class WhiteListService
{
    private readonly ERASettings _eraSettings;
    private readonly DomainService _domainService;
    private readonly EraService _eraService;
    private readonly LDAPSettings _ldapSettings;
    private readonly ValidationSettings _validationSettings;
    private readonly EraValidator _validator;
    private readonly ILogger<ValidationService> _logger;


    public WhiteListService(DomainService domainService,   EraService eraService, IOptions<ERASettings> eraSettings, IOptions<ValidationSettings> validationSettings, IOptions<LDAPSettings> ldapSettings)
    {
        _domainService = domainService;
        _eraService = eraService;
        _eraSettings = eraSettings.Value;
        _validationSettings = validationSettings.Value;
        _ldapSettings = ldapSettings.Value;
        _validator = new EraValidator(_domainService, _validationSettings);
    }
    
}