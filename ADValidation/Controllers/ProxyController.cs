// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;
// using System.Net.Http;
// using System.Threading.Tasks;
// using System.Linq;
// using System.Net.Http.Headers;
// using System.IO;
// using AspNetCore.Proxy.Builders;
//
// namespace ADValidation.Controllers
// {
//     [Route("proxy")]
//     [ApiController]
//     public class ProxyController : ControllerBase
//     {
//         private readonly AspNetCore.Proxy.Builders.Proxy _proxies;
//
//         public ProxyController(Proxy proxies)
//         {
//             _proxies = proxies;
//                 
//         }
//     
//         // The proxy route that forwards the requests
//         [HttpGet("{*url}")]
//         public Task<IActionResult> Get(string? url)
//         {
//             if (string.IsNullOrEmpty(url))
//             {
//                 return Task.FromResult<IActionResult>(BadRequest("The URL parameter is missing."));
//             }
//
//             // Build the target URL
//             var targetUrl = "https://www.ukr.net/" + url;
//
//             // Use the AspNetCore.Proxy middleware to forward the request to the target URL
//             return _proxies. RunProxy(targetUrl);
//         }
//     }
// }
