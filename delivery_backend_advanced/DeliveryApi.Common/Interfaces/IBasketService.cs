namespace delivery_backend_advanced.Services.Interfaces;

public interface IBasketService
{
    public Task AddDishToBasket(Guid dishId, Guid restaurantId);
}