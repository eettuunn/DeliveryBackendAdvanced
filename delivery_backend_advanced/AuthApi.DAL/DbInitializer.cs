using AuthApi.Common.ConfigClasses;
using AuthApi.Common.Enums;
using AuthApi.Common.Interfaces;
using AuthApi.DAL;
using AuthApi.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthApi.BL.Services;

public class DbInitializer : IDbInitializer
{
    private readonly AuthDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;

    public DbInitializer(AuthDbContext context, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, IConfiguration configuration)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
        _configuration = configuration;
    }

    public void InitializeAuthDb()
    {
        try
        {
            if (_context.Database.GetPendingMigrations().Any())
            {
                _context.Database.Migrate();
            }
        }
        catch (Exception)
        {
            //?
        }

        if (_roleManager.RoleExistsAsync("Customer").GetAwaiter().GetResult()) return;
        
        var adminConfig = _configuration.GetSection("AdminConfig").Get<AdminConfig>();

        _roleManager.CreateAsync(new IdentityRole("Customer")).GetAwaiter().GetResult();
        _roleManager.CreateAsync(new IdentityRole("Courier")).GetAwaiter().GetResult();
        _roleManager.CreateAsync(new IdentityRole("Manager")).GetAwaiter().GetResult();
        _roleManager.CreateAsync(new IdentityRole("Cook")).GetAwaiter().GetResult();
        _roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();

        var admin = new AppUser()
        {
            UserName = adminConfig.UserName,
            Email = adminConfig.Email,
            EmailConfirmed = true,
            BirthDate = DateTime.UtcNow,
            Gender = Gender.Male
        };
        _userManager.CreateAsync(admin, adminConfig.Password).GetAwaiter().GetResult();

        var user =
            _context.Users.FirstOrDefault(u => u.Email == adminConfig.Email);
        if (user != null)
        {
            _userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
            var adminEntity = new AdminEntity
            {
                Id = new Guid(),
                User = user
            };
            _context.Admins.Add(adminEntity);
            _context.SaveChanges();
        }
    }
}