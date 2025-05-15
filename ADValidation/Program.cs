using System.Text;
using ADValidation.Data;
using ADValidation.Decorators;
using ADValidation.Models;
using ADValidation.Models.Auth;
using ADValidation.Models.ERA;
using ADValidation.Services;
using ADValidation.Services.AccessPolicy;
using ADValidation.Services.Auth;
using ADValidation.Services.Validation;
using AspNetCore.Proxy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;

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

builder.Services.AddIdentity<ApplicationUser, IdentityRole<long>>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
        builder.AllowAnyOrigin()  // Allow any origin
            .AllowAnyHeader()  // Allow any headers
            .AllowAnyMethod()); // Allow any HTTP methods (GET, POST, etc.)
});

builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<AuditService>();
builder.Services.AddScoped<AuditLoggerService>();
builder.Services.AddScoped<AccessPolicyService>();

builder.Services.Configure<LDAPSettings>(builder.Configuration.GetSection("LDAPSettings"));
builder.Services.Configure<ERASettings>(builder.Configuration.GetSection("ERASettings"));
builder.Services.Configure<ValidationSettings>(builder.Configuration.GetSection("ValidationSettings"));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IPAddressService>();
builder.Services.AddScoped<ValidationService>();
builder.Services.AddScoped<DomainService>();
builder.Services.AddScoped<EraService>();
builder.Services.AddScoped<SeederService>();
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<SignInManager<ApplicationUser>>();

builder.Services.AddMemoryCache();
var app = builder.Build();

app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();    // This must come before UseAuthorization
app.UseAuthorization();     // This enables the [Authorize] attribute

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate(); // Apply migrations
        
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate(); // Apply migrations
        
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using (var scope = app.Services.CreateScope())
    {
        var seeder = scope.ServiceProvider.GetRequiredService<SeederService>(); 
        await seeder.Seed();
    }
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.WebRootPath, "browser", "assets")),
    RequestPath = "/assets"
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    
    // Fallback to Angular's index.html for client-side routing
    endpoints.MapFallbackToFile("/browser/index.html");
});


app.MapControllers()
    .WithOpenApi();

app.Run();