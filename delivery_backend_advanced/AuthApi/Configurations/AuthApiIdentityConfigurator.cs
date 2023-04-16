using AuthApi.DAL;
using AuthApi.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace delivery_backend_advanced.Configurations;

public static class AuthApiIdentityConfigurator
{
    public static void ConfigureAuthApiIdentity(this WebApplicationBuilder builder)
    {
        builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true; 
        
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddUserManager<UserManager<AppUser>>()
            .AddSignInManager<SignInManager<AppUser>>()
            .AddDefaultTokenProviders();
    }
}