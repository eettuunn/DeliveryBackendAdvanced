using AdminPanel.Interfaces;
using AdminPanel.Services;

namespace AdminPanel.Configurators;

public static class AdminPanelServicesConfigurator
{
    public static void ConfigureAdminPanelServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllersWithViews();

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

        builder.Services.AddAutoMapper(typeof(AppMappingProfile));
        
        builder.Services.AddScoped<IRestaurantService, RestaurantService>();
        builder.Services.AddScoped<IUserService, UserService>();
    }
}