using Microsoft.AspNetCore.SignalR;
using NotificationAPI.Configurators.ConfigClasses;
using NotificationAPI.Services;
using RabbitMQ.Client;

namespace NotificationAPI.Configurators;

public static class NotificationServicesConfigurator
{
    public static void ConfigureNotificationServices(this WebApplicationBuilder builder)
    {
        var rabbitMqConnection = builder.Configuration.GetSection("RabbitMqConnection").Get<RabbitMqConnection>();

        builder.Services.AddSignalR();
        builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();
        builder.Services.AddSingleton<IConnection>(x =>
            new ConnectionFactory
            {
                HostName = rabbitMqConnection.Hostname,
                UserName = rabbitMqConnection.Username,
                Password = rabbitMqConnection.Password,
                VirtualHost = rabbitMqConnection.VirtualHost
            }.CreateConnection()
        );
        builder.Services.AddHostedService<RabbitMqListener>();
    }
}