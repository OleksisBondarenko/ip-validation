using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using ADValidation.Models;
using ADValidation.Services;
using ADValidation.Utils;
using AspNetCore.Proxy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

var builder = WebApplication.CreateBuilder(args);

string SUCCESS_URL = builder.Configuration["ProxyURL"];
string ERROR_URL = builder.Configuration["ErrorURL"];
var ASSETS_URLS = builder.Configuration.GetSection("AssetURLS").Get<List<string>>();
string JS_FILE_URL = "/scripts/myScript.js"; // Adjust path to your JS file
string randomNonceValue = "randomNonceValue";

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddProxies();

// Configure HttpClient with custom settings
builder.Services
    .AddHttpClient("myClientName")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
    {
        UseDefaultCredentials = true,

        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true,
    });

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin() // Allow all origins
            .AllowAnyMethod() // Allow all HTTP methods (GET, POST, etc.)
            .AllowAnyHeader(); // Allow all headers
    });
});

builder.Services.Configure<LDAPSettings>(builder.Configuration.GetSection("LDAPSettings"));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IPAddressService>();
builder.Services.AddScoped<DomainService>();
builder.Services.AddScoped<ValidationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS middleware with "AllowAll" policy
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllers()
    .WithOpenApi();

app.UseEndpoints(endpoints =>
{
    // Map the ValidateController and other controllers here
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Validate}/{action=Index}/");
});

app.UseProxies(proxies =>
{
    proxies.Map("/{*url}",
        (proxy) =>
        {
            string proxyUrl = SUCCESS_URL;

            proxy.UseHttp((context, args) =>
            {
                string targetUrl = $"{proxyUrl}/{args["url"]}";
                
                // if (context.Request.Path.StartsWithSegments("/validate"))
                // {
                //     targetUrl = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}";
                //     return new ValueTask<string>(targetUrl);
                // }
                // using (var scope = app.Services.CreateScope())
                // {
                //     var services = scope.ServiceProvider;
                //
                //     IPAddressService ipAddressService = services.GetRequiredService<IPAddressService>();
                //     ValidationService validationService = services.GetRequiredService<ValidationService>();
                //
                //     string ipAddress = ipAddressService.GetRequestIP();
                //     string resolvedDns = DnsUtils.GetHostnameFromIp(ipAddress);
                //     bool isValidDNS = validationService.IsDNSPartOfDomain(resolvedDns);
                //
                //     proxyUrl = isValidDNS ? SUCCESS_URL : ERROR_URL;
                // }
                
                return new ValueTask<string>(targetUrl);
            }, options =>
            { 
                options.WithShouldAddForwardedHeaders(false);
                options.WithHttpClientName("myClientName");
                options.WithBeforeSend(async (context, request) =>
                {
                  
                });
                options.WithAfterReceive(async (context, response) =>
                {
                    string contentString = string.Empty;

                    if (response != null && 
                        response.Content != null && 
                        response.Content.Headers.ContentType != null)
                    {
                        var contentType = response.Content.Headers.ContentType.ToString();
                        string contentMediaType = string.Empty;
                        string contentMediaEncoding = string.Empty;

                        if (MediaTypeHeaderValue.TryParse(contentType, out MediaTypeHeaderValue mediaType))
                        {
                            contentMediaType = mediaType.MediaType;
                            contentMediaEncoding = mediaType.CharSet ?? "UTF-8";
                        }

                        if (contentMediaType.Equals("application/binary"))
                        {
                            return;
                        }
                        
                        if (response.Content.Headers.ContentEncoding != null &&
                            response.Content.Headers.ContentEncoding.Contains("gzip"))
                        {
                            var content = await response.Content.ReadAsByteArrayAsync();

                            using (var memoryStream = new MemoryStream(content))
                            using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                            using (var reader = new StreamReader(gzipStream, Encoding.UTF8))
                            {
                                contentString = await reader.ReadToEndAsync();
                            }
                        }
                        else
                        {
                            contentString = await response.Content.ReadAsStringAsync();
                        }

                        if (contentString.Contains("<html"))
                        {
                            // Add or modify Content-Security-Policy header
                            string cspHeader = $"script-src 'self' {string.Join(' ', ASSETS_URLS)} nonce-{randomNonceValue};";
                            if (response.Headers.Contains("Content-Security-Policy"))
                            {
                                response.Headers.Remove("Content-Security-Policy");
                            }

                            response.Headers.Add("Content-Security-Policy", cspHeader);

                            // Modify CSP to allow scripts from localhost:5006
                            string cspMetaTag =
                                $"<meta http-equiv=\"Content-Security-Policy\" content=\"script-src 'self' 'unsafe-inline' {string.Join(' ', ASSETS_URLS)};\">";
                            //"<meta http-equiv=\"Content-Security-Policy\" content=\"default-src * self blob: data: gap:; style-src * self 'unsafe-inline' blob: data: gap:; script-src * 'self' 'unsafe-eval' 'unsafe-inline' blob: data: gap:; object-src * 'self' blob: data: gap:; img-src * self 'unsafe-inline' blob: data: gap:; connect-src self * 'unsafe-inline' blob: data: gap:; frame-src * self blob: data: gap:;\">";

                            contentString = Regex.Replace(contentString, "<head>", $"<head>{cspMetaTag}",
                                RegexOptions.IgnoreCase);

                            // Inject your script tag for the JavaScript file
                            string scriptTag = $"<script nonce={randomNonceValue} src='http://localhost:5006/scripts/myScript.js'></script>";
                            contentString = Regex.Replace(contentString, "</body>", scriptTag + "</body>",
                                RegexOptions.IgnoreCase);
                            
                            // Update the response content
                            response.Content = new StringContent(contentString,
                                Encoding.GetEncoding(contentMediaEncoding), contentMediaType);
                        }
                    }
                });
            });
        });
});


app.Run();