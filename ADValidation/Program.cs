using System.Net;
using ADValidation.Models;
using ADValidation.Services;
using ADValidation.Utils;
using AspNetCore.Proxy;
using AspNetCore.Proxy.Builders;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

string SUCCESS_URL = builder.Configuration["ProxyURL"];
string ERROR_URL = builder.Configuration["ErrorURL"];

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddProxies();

builder.Services
    .AddHttpClient("myClientName")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler() {
        UseDefaultCredentials = true,
        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true, // Always accept the certificate
    });


// In Program.cs or Startup.cs
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

app.UseStaticFiles(); // This will serve the js files in wwwroot
app.UseHttpsRedirection();

app.UseRouting();



app.MapControllers()
    .WithOpenApi();

app.UseProxies(proxies =>
{
    proxies.Map("/{*url}",
        (proxy) =>
        {
            string proxyUrl = "";

            proxy.UseHttp((_, args) =>
                {
                    using (var scope = app.Services.CreateScope())
                    {
                        var services = scope.ServiceProvider;

                        IPAddressService ipAddressService = services.GetRequiredService<IPAddressService>();
                        ValidationService validationService = services.GetRequiredService<ValidationService>();

                        string ipAddress = ipAddressService.GetRequestIP();
                        string resolvedDns = DnsUtils.GetHostnameFromIp(ipAddress);
                        bool isValidDNS = validationService.IsDNSPartOfDomain(resolvedDns);

                        proxyUrl = isValidDNS ? SUCCESS_URL : ERROR_URL;
                    }

                    return new ValueTask<string>($"{proxyUrl}{args["url"]}");
                }
                , options =>
                {
                        // Configure the HTTP proxy options builder to allow unsafe HTTPS connections.
                        options.WithHttpClientName("myClientName");
                });
        });
});


app.Run();