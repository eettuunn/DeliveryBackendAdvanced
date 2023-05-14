using delivery_backend_advanced.Models.Dtos;

namespace delivery_backend_advanced.Services.Interfaces;

public interface IBasketService
{
    public Task AddDishToBasket(Guid dishId, Guid restaurantId, CustomerInfoDto customerInfoDto);
    public Task<BasketDto> GetUserBasket(Guid userId);
    public Task DeleteDishFromBasket(Guid dishBasketId, Guid userId);
    public Task ReduceDishInBasket(Guid dishBasketId, Guid userId);
}