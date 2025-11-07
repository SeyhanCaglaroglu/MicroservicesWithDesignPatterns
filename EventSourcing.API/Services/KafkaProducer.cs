using Confluent.Kafka;
using EventSourcing.API.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace EventSourcing.API.Services;

public class KafkaProducer : IKafkaProducer, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly KafkaSettings _kafkaSettings;
    private readonly ILogger<KafkaProducer> _logger;

    public KafkaProducer(IOptions<KafkaSettings> kafkaSettings, ILogger<KafkaProducer> logger)
    {
        _kafkaSettings = kafkaSettings.Value;
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true,
            MessageTimeoutMs = 30000
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync<T>(string topic, T message, string? key = null)
    {

        var serializedMessage = JsonSerializer.Serialize(message, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var kafkaMessage = new Message<string, string>
        {
            Key = key ?? Guid.NewGuid().ToString(),
            Value = serializedMessage,
            Headers = new Headers
                {
                    { "MessageType", System.Text.Encoding.UTF8.GetBytes(typeof(T).Name) },
                    { "Timestamp", System.Text.Encoding.UTF8.GetBytes(DateTimeOffset.UtcNow.ToString("O")) }
                }
        };

        var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage);

        _logger.LogInformation("Message published to Kafka. Topic: {Topic}, Partition: {Partition}, Offset: {Offset}",
            deliveryResult.Topic, deliveryResult.Partition, deliveryResult.Offset);


    }

    public void Dispose()
    {
        _producer?.Flush(TimeSpan.FromSeconds(10));
        _producer?.Dispose();
    }
}