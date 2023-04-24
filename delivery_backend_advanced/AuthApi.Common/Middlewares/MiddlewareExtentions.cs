using Microsoft.AspNetCore.Builder;

namespace AuthApi.Common.Middlewares;

public static class MiddlewareExtentions
{
    public static void UseLoggerMiddleware(this WebApplication app)
    {
        app.UseMiddleware<LoggerMiddleware>();
    }
}