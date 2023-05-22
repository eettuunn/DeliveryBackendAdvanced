using Microsoft.AspNetCore.SignalR;
using NotificationAPI.Services;
using RabbitMQ.Client;

namespace NotificationAPI.Configurators;

public static class NotificationServicesConfigurator
{
    public static void ConfigureNotificationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSignalR();
        builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();
        builder.Services.AddSingleton<IConnection>(x =>
            new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "user",
                Password = "1234",
                VirtualHost = "/"
            }.CreateConnection()
        );
        builder.Services.AddHostedService<RabbitMqListener>();
    }
}