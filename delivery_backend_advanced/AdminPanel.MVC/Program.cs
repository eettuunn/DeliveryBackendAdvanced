using AdminPanel.Configurators;
using AdminPanel.Interfaces;
using AdminPanel.Services;
using AuthApi.DAL;
using AuthApi.DAL.Entities;
using delivery_backend_advanced.Configurations;
using DeliveryApi.DAL;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);


builder.ConfigureIdentity();

builder.ConfigureAdminPanelServices();

builder.ConfigureDeliveryApiDAL();
builder.ConfigureAuthDAL();

var app = builder.Build();

app.ConfigureDeliveryApiDAL();
app.ConfigureAuthDAL();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=StartPage}/{id?}");

app.Run();