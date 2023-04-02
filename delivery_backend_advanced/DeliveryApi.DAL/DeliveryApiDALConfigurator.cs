using delivery_backend_advanced.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DeliveryApi.DAL;

public static class DeliveryApiDALConfigurator
{
    public static void ConfigureDeliveryApiDAL(this WebApplicationBuilder builder)
    {
        var connection = builder.Configuration.GetConnectionString("Postgres");
        builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connection));
    }

    public static void ConfigureDeliveryApiDAL(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
            dbContext?.Database.Migrate();
        }
    }
}