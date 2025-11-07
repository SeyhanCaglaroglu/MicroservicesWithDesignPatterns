namespace EventSourcing.API.Services;

public interface IKafkaProducer
{
    Task PublishAsync<T>(string topic, T message, string? key = null);
}