using System.Text;
using AuthApi.Common.ConfigClasses;
using delivery_backend_advanced.Policies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace AdminPanel.Configurators;

public static class JwtInCookiesConfigurator
{
    public static void ConfigureJwtInCookies(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.Key))
                };
                
                options.Events = new JwtBearerEvents
                {
                    //Берем токен из кукис и добавляем в запрос
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["access_token"];
                        return Task.CompletedTask;
                    },
                    
                    OnChallenge = context => 
                    {
                        context.Response.Redirect("/Account/Login");
                        context.HandleResponse();
                        return Task.FromResult(0);
                    },
                    
                    OnForbidden = context =>
                    {
                        context.Response.Cookies.Delete("access_token");
                        context.Response.Redirect("/Account/Login");
                        return Task.FromResult(0);
                    }
                };
            });
        builder.Services.AddAuthorization(options =>
        {
            options.DefaultPolicy =
                new AuthorizationPolicyBuilder
                        (JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            options.AddPolicy(
                "Ban",
                policy => policy.Requirements.Add(new BanPolicy()));
        });
    }
}