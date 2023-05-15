using delivery_backend_advanced.Models.Dtos;

namespace delivery_backend_advanced.Services.Interfaces;

public interface ICookService
{
    public Task TakeOrder(Guid orderId, UserInfoDto userInfoDto);

    public Task ChangeOrderStatus(Guid orderId, UserInfoDto userInfoDto);
}