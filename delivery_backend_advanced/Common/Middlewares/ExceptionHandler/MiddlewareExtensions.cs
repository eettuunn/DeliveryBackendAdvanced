using AuthApi.Common.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace delivery_backend_advanced.Services.ExceptionHandler;

public static class MiddlewareExtensions
{
    public static void UseExceptionMiddleware(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddlewareService>();
    }
    
    public static void UseLoggerMiddleware(this WebApplication app)
    {
        app.UseMiddleware<LoggerMiddleware>();
    }
}