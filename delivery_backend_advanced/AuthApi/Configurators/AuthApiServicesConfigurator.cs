using System.Reflection;
using AuthApi.BL;
using AuthApi.BL.Services;
using AuthApi.Common.Interfaces;
using delivery_backend_advanced.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.OpenApi.Models;

namespace delivery_backend_advanced.Configurations;

public static class AuthApiServicesConfigurator
{
    public static void ConfigureAuthApiServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IProfileService, ProfileService>();
        builder.Services.AddScoped<IDbAuthInitializer, DbAuthAuthInitializer>();
        builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Services.AddSingleton<IAuthorizationHandler, BanPolicyHandler>();

        builder.Services.AddAutoMapper(typeof(AppMappingProfile));
    }
}