using AdminPanel.Interfaces;
using AdminPanel.Services;
using DeliveryApi.DAL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddAutoMapper(typeof(AppMappingProfile));

builder.Services.AddScoped<IRestaurantService, RestaurantService>();

builder.ConfigureDeliveryApiDAL();

var app = builder.Build();

app.ConfigureDeliveryApiDAL();

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
    pattern: "{controller=Restaurant}/{action=CreateRest}/{id?}");

app.Run();