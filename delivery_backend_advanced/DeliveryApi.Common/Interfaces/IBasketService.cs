using delivery_backend_advanced.Models.Dtos;

namespace delivery_backend_advanced.Services.Interfaces;

public interface IBasketService
{
    public Task AddDishToBasket(Guid dishId, Guid restaurantId);
    public Task<BasketDto> GetUserBasket();

    public Task DeleteDishFromBasket(Guid dishBasketId);
}