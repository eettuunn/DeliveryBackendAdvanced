using delivery_backend_advanced.Models.Dtos;

namespace delivery_backend_advanced.Services.Interfaces;

public interface IManagerService
{
    public Task CreateMenu(CreateMenuDto createMenuDto);
}