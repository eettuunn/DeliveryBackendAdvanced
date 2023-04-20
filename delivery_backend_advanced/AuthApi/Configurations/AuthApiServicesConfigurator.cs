using System.Reflection;
using AuthApi.BL;
using AuthApi.BL.Services;
using AuthApi.Common.Interfaces;
using Microsoft.OpenApi.Models;

namespace delivery_backend_advanced.Configurations;

public static class AuthApiServicesConfigurator
{
    public static void ConfigureAuthApiServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IProfileService, ProfileService>();

        builder.Services.AddAutoMapper(typeof(AppMappingProfile));
    }
}