using delivery_backend_advanced.Models.Dtos;

namespace delivery_backend_advanced.Services.Interfaces;

public interface IOrderService
{
    public Task CreateOrder(CreateOrderDto orderDto);

    public Task<List<OrderListElementDto>> GetUserOrders();
    
    public Task<OrderDto> GetOrderDetails(Guid orderId);

    public Task CancelOrder(Guid orderId);
}