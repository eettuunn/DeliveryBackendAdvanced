using System.Text;
using AuthApi.Common.ConfigClasses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using NotificationAPI;
using NotificationAPI.Configurators;
using NotificationAPI.Hubs;
using NotificationAPI.Services;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithOrigins("http://localhost:63343");
    });
});

builder.ConfigureNotificationServices();

builder.ConfigureNotificationAuth();

builder.Services.AddControllersWithViews();

builder.ConfigureNotificationDbContext();

var app = builder.Build();

app.ConfigureNotificationDbContext();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationsHub>("/notifications");

app.Run();