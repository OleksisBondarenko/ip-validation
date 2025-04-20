using ADValidation.Decorators;
using ADValidation.Models;
using ADValidation.Models.ERA;
using ADValidation.Services;
using AspNetCore.Proxy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
        builder.AllowAnyOrigin()  // Allow any origin
            .AllowAnyHeader()  // Allow any headers
            .AllowAnyMethod()); // Allow any HTTP methods (GET, POST, etc.)
});


builder.Services.AddScoped<AuditService>();
builder.Services.AddScoped<AuditLoggerService>();

builder.Services.Configure<LDAPSettings>(builder.Configuration.GetSection("LDAPSettings"));
builder.Services.Configure<ERASettings>(builder.Configuration.GetSection("ERASettings"));
builder.Services.Configure<ValidationSettings>(builder.Configuration.GetSection("ValidationSettings"));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IPAddressService>();
builder.Services.AddScoped<DomainService>();
builder.Services.AddScoped<EraService>();
builder.Services.AddMemoryCache();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate(); // Apply migrations
}

app.UseCors("AllowAllOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    
    // Fallback to Angular's index.html for client-side routing
    endpoints.MapFallbackToFile("/browser/index.html");
});

app.MapControllers()
    .WithOpenApi();

app.Run();