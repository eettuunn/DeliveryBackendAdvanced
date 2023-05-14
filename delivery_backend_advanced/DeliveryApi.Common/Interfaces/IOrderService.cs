using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Models.Enums;

namespace delivery_backend_advanced.Services.Interfaces;

public interface IOrderService
{
    public Task CreateOrder(CreateOrderDto orderDto, CustomerInfoDto customerInfoDto);

    public Task<OrdersPageDto> GetOrders(OrderQueryModel orderQueryModel, UserRole roles, CustomerInfoDto customerInfoDto);
    
    public Task<OrderDto> GetOrderDetails(Guid orderId, CustomerInfoDto customerInfoDto);

    public Task CancelOrder(Guid orderId, CustomerInfoDto customerInfoDto);

    public Task RepeatOrder(RepeatOrderDto repOrder, Guid orderId, CustomerInfoDto customerInfoDto);
}