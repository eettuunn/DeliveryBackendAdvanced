namespace delivery_backend_advanced.Services.Interfaces;

public interface ICookService
{
    public Task TakeOrder(Guid orderId);
}