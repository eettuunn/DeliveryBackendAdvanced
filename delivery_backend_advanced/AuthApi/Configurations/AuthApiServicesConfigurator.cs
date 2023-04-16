using System.Reflection;
using AuthApi.BL;
using AuthApi.BL.Services;
using AuthApi.Common.Interfaces;

namespace delivery_backend_advanced.Configurations;

public static class AuthApiServicesConfigurator
{
    public static void ConfigureAuthApiServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{ Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IProfileService, ProfileService>();

        builder.Services.AddAutoMapper(typeof(AppMappingProfile));
    }
}