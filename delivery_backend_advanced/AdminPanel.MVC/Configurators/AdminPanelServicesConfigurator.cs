using AdminPanel.Interfaces;
using AdminPanel.Services;
using AuthApi.BL.Services;
using AuthApi.Common.Interfaces;
using delivery_backend_advanced.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AdminPanel.Configurators;

public static class AdminPanelServicesConfigurator
{
    public static void ConfigureAdminPanelServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllersWithViews();

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

        builder.Services.AddAutoMapper(typeof(AppMappingProfile));
        
        builder.Services.AddScoped<IDbAuthInitializer, DbAuthAuthInitializer>();
        builder.Services.AddScoped<IRestaurantService, RestaurantService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        
        builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Services.AddSingleton<IAuthorizationHandler, BanPolicyHandler>();
    }
}