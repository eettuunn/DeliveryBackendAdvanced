using delivery_backend_advanced.Models.Dtos;

namespace delivery_backend_advanced.Services.Interfaces;

public interface IOrderService
{
    public Task CreateOrder(CreateOrderDto orderDto);
}