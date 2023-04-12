using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthApi.DAL;

public static class AuthApiDALConfigurator
{
    public static void ConfigureDeliveryApiDAL(this WebApplicationBuilder builder)
    {
        var connection = builder.Configuration.GetConnectionString("Postgres");
        builder.Services.AddDbContext<AuthDbContext>(options => options.UseNpgsql(connection));
    }

    public static void ConfigureDeliveryApiDAL(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetService<AuthDbContext>();
            dbContext?.Database.Migrate();
        }
    }
}