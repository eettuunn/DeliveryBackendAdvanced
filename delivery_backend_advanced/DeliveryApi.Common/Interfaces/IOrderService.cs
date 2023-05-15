using delivery_backend_advanced.Models.Dtos;
using delivery_backend_advanced.Models.Enums;

namespace delivery_backend_advanced.Services.Interfaces;

public interface IOrderService
{
    public Task CreateOrder(CreateOrderDto orderDto, UserInfoDto userInfoDto);

    public Task<OrdersPageDto> GetOrders(OrderQueryModel orderQueryModel, UserRole roles, UserInfoDto userInfoDto);
    
    public Task<OrderDto> GetOrderDetails(Guid orderId, UserInfoDto userInfoDto);

    public Task CancelOrder(Guid orderId, UserInfoDto userInfoDto);

    public Task RepeatOrder(RepeatOrderDto repOrder, Guid orderId, UserInfoDto userInfoDto);
}