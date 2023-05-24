using System.Net;
using System.Text;
using AdminPanel.Configurators;
using AdminPanel.Interfaces;
using AdminPanel.Services;
using AuthApi.Common.ConfigClasses;
using AuthApi.DAL;
using AuthApi.DAL.Entities;
using Common.Configurations;
using delivery_backend_advanced.Configurations;
using delivery_backend_advanced.Policies;
using DeliveryApi.DAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


builder.ConfigureIdentity();

builder.ConfigureAdminPanelServices();

builder.ConfigureDeliveryApiDAL();
builder.ConfigureAuthDAL();

builder.ConfigureJwtInCookies();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();