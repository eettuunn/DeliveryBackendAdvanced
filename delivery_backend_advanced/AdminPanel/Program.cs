using AdminPanel.Interfaces;
using AdminPanel.Services;
using AuthApi.DAL;
using DeliveryApi.DAL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddAutoMapper(typeof(AppMappingProfile));

builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<IUserService, UserService>();

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