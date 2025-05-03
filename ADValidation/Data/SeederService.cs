using ADValidation.Models.Auth;
using Microsoft.AspNetCore.Identity;

namespace ADValidation.Data;

public class SeederService
{
    private ApplicationDbContext _context;
    private UserManager<ApplicationUser> _userManager;

    public SeederService(
        ApplicationDbContext context, 
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
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
    }
}