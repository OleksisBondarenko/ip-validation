using ADValidation.Decorators;
using ADValidation.Models;
using ADValidation.Models.ERA;
using ADValidation.Services;
using AspNetCore.Proxy;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddProxies();

// Add DbContext with SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<AuditService>();
builder.Services.AddScoped<AuditLoggerService>();

builder.Services.Configure<LDAPSettings>(builder.Configuration.GetSection("LDAPSettings"));
builder.Services.Configure<ERASettings>(builder.Configuration.GetSection("ERASettings"));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IPAddressService>();
builder.Services.AddScoped<DomainService>();

builder.Services.AddScoped<EraValidationService>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapControllers()
    .WithOpenApi();

app.Run();