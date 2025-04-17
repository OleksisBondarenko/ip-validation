// using ADValidation.Models;
//
// namespace ADValidation.Services.Validation;
//
// public class ValidationService
// {
//     
//     private readonly DomainService _domainService;
//     private readonly EraService _eraService;
//
//     public ValidationService(DomainService domainService,   EraService eraService)
//     {
//         _domainService = domainService;
//         _eraService = eraService;
//     }
//
//     public async Task<ValidationSuccessResult> ValidateAsync(string ipAddress)
//     {
//         var aggregatedData = await _eraService.GetComputerAggregatedData(ipAddress);
//         
//         string domain = _domainService.GetDomainFromHostname(aggregatedData.ComputerName);
//         string hostname = aggregatedData.ComputerName;
//         
//         return new ValidationSuccessResult()
//         {
//             IpAddress = ipAddress,
//             Domain = domain,
//             Hostname =  hostname,
//         };
//     }
// }