using System.Reflection;
using System.Text.Json.Serialization;
using delivery_backend_advanced.Services;
using delivery_backend_advanced.Services.Interfaces;
using DeliveryApi.BL.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace delivery_backend_advanced.Models;

public static class DeliveryApiBLConfigurator
{
    public static void ConfigureDeliveryApiServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper(typeof(AppMappingProfile));
        
        builder.Services.AddScoped<IRestaurantService, RestaurantService>();
        builder.Services.AddScoped<IDishService, DishService>();
        builder.Services.AddScoped<IBasketService, BasketService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<ICookService, CookService>();
        builder.Services.AddScoped<ICourierService, CourierService>();
        builder.Services.AddScoped<IManagerService, ManagerService>();
        
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{ Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
        
        builder.Services
            .AddControllers()
            .AddJsonOptions(options => 
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    }
}