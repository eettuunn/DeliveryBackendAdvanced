using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Models.Enums;

namespace delivery_backend_advanced.Services.Interfaces;

public interface IOrderService
{
    public Task CreateOrder(CreateOrderDto orderDto);

    public Task<OrdersPageDto> GetOrders(OrderQueryModel orderQueryModel, UserRole roles);
    
    public Task<OrderDto> GetOrderDetails(Guid orderId);

    public Task CancelOrder(Guid orderId);

    public Task RepeatOrder(RepeatOrderDto repOrder, Guid orderId);
}