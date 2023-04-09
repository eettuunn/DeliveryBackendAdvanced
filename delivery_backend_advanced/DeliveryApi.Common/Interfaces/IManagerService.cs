using delivery_backend_advanced.Models.Dtos;

namespace delivery_backend_advanced.Services.Interfaces;

public interface IManagerService
{
    public Task CreateMenu(CreateMenuDto createMenuDto);
    public Task AddDishToMenu(Guid menuId, Guid dishId);
    public Task DeleteDishFromMenu(Guid menuId, Guid dishId);
}