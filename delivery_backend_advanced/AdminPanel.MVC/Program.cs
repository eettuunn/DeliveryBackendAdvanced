using AdminPanel.Configurators;
using AuthApi.DAL;
using delivery_backend_advanced.Configurations;
using DeliveryApi.DAL;

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