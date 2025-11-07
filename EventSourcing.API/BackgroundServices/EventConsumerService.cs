using Confluent.Kafka;
using EventSourcing.API.Models;
using EventSourcing.Shared.Events;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace EventSourcing.API.BackgroundServices;

public class EventConsumerService : BackgroundService
{
    private readonly ILogger<EventConsumerService> _logger;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IServiceProvider _serviceProvider;
    private IConsumer<string, string>? _consumer;

    public EventConsumerService(
        ILogger<EventConsumerService> logger,
        IOptions<KafkaSettings> kafkaSettings,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _kafkaSettings = kafkaSettings.Value;
        _serviceProvider = serviceProvider;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _consumer?.Close();
        _consumer?.Dispose();
        return base.StopAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaSettings.BootstrapServers,
                GroupId = _kafkaSettings.GroupId,
                AutoOffsetReset = Enum.Parse<AutoOffsetReset>(_kafkaSettings.AutoOffsetReset),
                EnableAutoCommit = _kafkaSettings.EnableAutoCommit
            };

            _consumer = new ConsumerBuilder<string, string>(config).Build();
            _consumer.Subscribe(_kafkaSettings.ProductTopic);

            _logger.LogInformation("EventConsumerService started and listening to topic: {Topic}", _kafkaSettings.ProductTopic);

            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(TimeSpan.FromMilliseconds(100));

                if (consumeResult?.Message != null)
                {
                    await EventAppeared(consumeResult);
                }
            }

            _logger.LogInformation("EventConsumerService stopped");
        }, stoppingToken);
    }

    private async Task EventAppeared(ConsumeResult<string, string> consumeResult)
    {
        var eventData = consumeResult.Message.Value;
        Type? type = null;

        // Try to get event type from header if it exists
        var messageTypeHeader = consumeResult.Message.Headers.FirstOrDefault(h => h.Key == "MessageType");
        if (messageTypeHeader != null)
        {
            var messageTypeName = System.Text.Encoding.UTF8.GetString(messageTypeHeader.GetValueBytes());
            type = Type.GetType($"EventSourcing.Shared.Events.{messageTypeName}, EventSourcing.Shared");
        }

        // If header didn't work, determine type from JSON content
        if (type == null)
        {
            type = DetermineEventType(eventData);
        }

        _logger.LogInformation("The Message processing... : {EventType}", type.Name);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var @event = JsonSerializer.Deserialize(eventData, type, jsonOptions);

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Product? product = null;

        switch (@event)
        {
            case ProductCreatedEvent productCreatedEvent:
                product = new Product()
                {
                    Id = productCreatedEvent.Id,
                    Name = productCreatedEvent.Name,
                    Price = productCreatedEvent.Price,
                    Stock = productCreatedEvent.Stock,
                    UserId = productCreatedEvent.UserId
                };
                context.Products.Add(product);
                break;

            case ProductNameChangedEvent productNameChangedEvent:
                product = context.Products.Find(productNameChangedEvent.Id);
                if (product != null)
                {
                    product.Name = productNameChangedEvent.ChangedName;
                }
                break;

            case ProductPriceChangedEvent productPriceChangedEvent:
                product = context.Products.Find(productPriceChangedEvent.Id);
                if (product != null)
                {
                    product.Price = productPriceChangedEvent.ChangedPrice;
                }
                break;

            case ProductDeletedEvent productDeletedEvent:
                product = context.Products.Find(productDeletedEvent.Id);
                if (product != null)
                {
                    context.Products.Remove(product);
                }
                break;
        }

        await context.SaveChangesAsync();

        if (!_kafkaSettings.EnableAutoCommit)
        {
            _consumer?.Commit(consumeResult);
        }
    }

    private Type DetermineEventType(string eventData)
    {
        using var jsonDocument = JsonDocument.Parse(eventData);
        var root = jsonDocument.RootElement;

        if ((root.TryGetProperty("name", out _) || root.TryGetProperty("Name", out _)) &&
            (root.TryGetProperty("price", out _) || root.TryGetProperty("Price", out _)) &&
            (root.TryGetProperty("stock", out _) || root.TryGetProperty("Stock", out _)))
        {
            return typeof(ProductCreatedEvent);
        }
        else if (root.TryGetProperty("changedName", out _) || root.TryGetProperty("ChangedName", out _))
        {
            return typeof(ProductNameChangedEvent);
        }
        else if (root.TryGetProperty("changedPrice", out _) || root.TryGetProperty("ChangedPrice", out _))
        {
            return typeof(ProductPriceChangedEvent);
        }
        else
        {
            return typeof(ProductDeletedEvent);
        }
    }
}
