using delivery_backend_advanced.Models.Dtos;

namespace delivery_backend_advanced.Services.Interfaces;

public interface ICourierService
{
    public Task SetOrderDelivered(Guid orderId, UserInfoDto userInfoDto);
    public Task TakeOrder(Guid orderId, UserInfoDto userInfoDto);
    public Task CancelOrder(Guid orderId, UserInfoDto userInfoDto);
}