namespace delivery_backend_advanced.Services.Interfaces;

public interface IMessageProducer
{
    public void SendMessage<T>(T message);
}