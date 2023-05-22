using Microsoft.EntityFrameworkCore;

namespace NotificationAPI;

public static class NotificationDbContextConfigurator
{
    public static void ConfigureNotificationDbContext(this WebApplicationBuilder builder)
    {
        var connection = builder.Configuration.GetConnectionString("PostgresBackend");
        builder.Services.AddDbContext<NotificationDbContext>(options => options.UseNpgsql(connection));
    }

    public static void ConfigureNotificationDbContext(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetService<NotificationDbContext>();
            dbContext?.Database.Migrate();
        }
    }
}