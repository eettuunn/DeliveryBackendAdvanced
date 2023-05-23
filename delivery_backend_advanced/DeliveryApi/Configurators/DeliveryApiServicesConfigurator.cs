using System.Text.Json.Serialization;
using delivery_backend_advanced.Policies;
using delivery_backend_advanced.Services;
using delivery_backend_advanced.Services.Interfaces;
using DeliveryApi.BL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NotificationAPI.Configurators.ConfigClasses;
using RabbitMQ.Client;

namespace delivery_backend_advanced.Models;

public static class DeliveryApiBLConfigurator
{
    public static void ConfigureDeliveryApiServices(this WebApplicationBuilder builder)
    {
        var rabbitMqConnection = builder.Configuration.GetSection("RabbitMqConnection").Get<RabbitMqConnection>();

        builder.Services.AddAutoMapper(typeof(AppMappingProfile));
        
        builder.Services.AddScoped<IRestaurantService, RestaurantService>();
        builder.Services.AddScoped<IDishService, DishService>();
        builder.Services.AddScoped<IBasketService, BasketService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<ICookService, CookService>();
        builder.Services.AddScoped<ICourierService, CourierService>();
        builder.Services.AddScoped<IManagerService, ManagerService>();
        builder.Services.AddScoped<IMessageProducer, MessageProducer>();
        builder.Services.AddSingleton<IConnection>(x =>
            new ConnectionFactory
            {
                HostName = rabbitMqConnection.Hostname,
                UserName = rabbitMqConnection.Username,
                Password = rabbitMqConnection.Password,
                VirtualHost = rabbitMqConnection.VirtualHost
            }.CreateConnection()
        );
        
        builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Services.AddSingleton<IAuthorizationHandler, BanPolicyHandler>();

        
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        
        builder.Services
            .AddControllers()
            .AddJsonOptions(options => 
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    }
}