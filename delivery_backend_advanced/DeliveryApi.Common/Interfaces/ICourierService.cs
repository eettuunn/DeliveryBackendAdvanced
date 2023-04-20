namespace delivery_backend_advanced.Services.Interfaces;

public interface ICourierService
{
    public Task SetOrderDelivered(Guid orderId);
    public Task TakeOrder(Guid orderId);
    public Task CancelOrder(Guid orderId);
}