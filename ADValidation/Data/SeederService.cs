using System.Data.Common;
using ADValidation.Enums;
using ADValidation.Helpers.Ip;
using ADValidation.Models;
using ADValidation.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ADValidation.Data;

public class SeederService
{
    private ApplicationDbContext _context;
    private readonly ValidationSettings _validationSettings;
    private UserManager<ApplicationUser> _userManager;

    public SeederService(
        ApplicationDbContext context, 
        UserManager<ApplicationUser> userManager,
        IOptions<ValidationSettings> validationSettings
        )
    {
        _context = context;
        _validationSettings = validationSettings.Value;
        _userManager = userManager;
    }
        
    public async Task Seed()
    {
        _context.Database.EnsureCreated();

        if (!_userManager.Users.Any());
        {
            ApplicationUser user = new ApplicationUser() { UserName = "admin", Email = "Admin1@admin.com", EmailConfirmed = true, PhoneNumberConfirmed = true};
           
            var result = await _userManager.CreateAsync(user, "Admin1@admin.com");
        }

        if (!_context.AccessPolicies.Any())
        {
            AccessPolicy localAllowPolicy = new AccessPolicy()
            {   
                Name = "Local allow policy",
                Description = "Just test local access policy",
                IsActive = true,
                Order = 0,
                Action = AccessAction.Allow,
                IpFilterRules = new [] { "127.0.0.1" },
                 PolicyEndDatetime = DateTime.UtcNow.AddYears(1),
                 PolicyStartDatetime = DateTime.UtcNow.AddDays(-1),
            };
            
            AccessPolicy localBlockPolicy = new AccessPolicy()
            {   
                Name = "Local block policy",
                Description = "Just test local access policy",
                IsActive = true,
                Order = 1,
                Action = AccessAction.Allow,
                IpFilterRules = new [] { "127.0.0.1" },
                PolicyEndDatetime = DateTime.UtcNow.AddYears(1),
                PolicyStartDatetime = DateTime.UtcNow.AddDays(-1),
            };
            
            _context.AccessPolicies.Add(PolicyFromConfig());
            _context.AccessPolicies.Add(localAllowPolicy);
            _context.AccessPolicies.Add(localBlockPolicy);

            try
            {
                _context.SaveChanges();
            }
            catch (DbException ex)
            {
                throw ex;
            }
        }
    }
    private AccessPolicy PolicyFromConfig ()
    {
        var whiteListReader = new WhiteListIpConfigReader(_validationSettings.WhiteListConfigPath);
        var whiteListIps = whiteListReader.WhiteListIPs();

        return new AccessPolicy()
        {
            Action = AccessAction.Allow,
            Order = 1,
            IpFilterRules = whiteListIps,
            IsActive = true,
            // ValidationTypes = new List<ValidatorType> { ValidatorType.Ip },
        };
    }
}