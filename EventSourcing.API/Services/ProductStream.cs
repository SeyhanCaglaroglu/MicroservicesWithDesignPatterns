using EventSourcing.API.Dtos;
using EventSourcing.API.Models;
using EventSourcing.Shared.Events;
using Microsoft.Extensions.Options;

namespace EventSourcing.API.Services;

public class ProductStream
{
    private readonly IKafkaProducer _kafkaProducer;
    private readonly KafkaSettings _kafkaSettings;
    private readonly ILogger<ProductStream> _logger;

    public ProductStream(IKafkaProducer kafkaProducer, IOptions<KafkaSettings> kafkaSettings, ILogger<ProductStream> logger)
    {
        _kafkaProducer = kafkaProducer;
        _kafkaSettings = kafkaSettings.Value;
        _logger = logger;
    }

    public async Task Created(CreateProductDto dto)
    {

        var productId = Guid.NewGuid();
        var correlationId = Guid.NewGuid().ToString();

        var productCreatedEvent = new ProductCreatedEvent
        {
            Id = productId,
            Name = dto.Name,
            Price = dto.Price,
            Stock = dto.Stock,
            UserId = dto.UserId
        };

        await _kafkaProducer.PublishAsync(_kafkaSettings.ProductTopic, productCreatedEvent, correlationId);

        _logger.LogInformation("ProductCreatedEvent published for Product ID: {ProductId}, Correlation ID: {CorrelationId}",
            productId, correlationId);

    }

    public async Task NameChanged(ChangeProductNameDto dto)
    {

        var correlationId = Guid.NewGuid().ToString();

        var productNameChangedEvent = new ProductNameChangedEvent
        {
            Id = dto.Id,
            ChangedName = dto.Name
        };

        await _kafkaProducer.PublishAsync(_kafkaSettings.ProductTopic, productNameChangedEvent, correlationId);

        _logger.LogInformation("ProductNameChangedEvent published for Product ID: {ProductId}, Correlation ID: {CorrelationId}",
            dto.Id, correlationId);

    }

    public async Task PriceChanged(ChangeProductPriceDto dto)
    {

        var correlationId = Guid.NewGuid().ToString();

        var productPriceChangedEvent = new ProductPriceChangedEvent
        {
            Id = dto.Id,
            ChangedPrice = dto.Price
        };

        await _kafkaProducer.PublishAsync(_kafkaSettings.ProductTopic, productPriceChangedEvent, correlationId);

        _logger.LogInformation("ProductPriceChangedEvent published for Product ID: {ProductId}, Correlation ID: {CorrelationId}",
            dto.Id, correlationId);


    }

    public async Task Deleted(Guid productId)
    {

        var correlationId = Guid.NewGuid().ToString();

        var productDeletedEvent = new ProductDeletedEvent
        {
            Id = productId
        };

        await _kafkaProducer.PublishAsync(_kafkaSettings.ProductTopic, productDeletedEvent, correlationId);

        _logger.LogInformation("ProductDeletedEvent published for Product ID: {ProductId}, Correlation ID: {CorrelationId}",
            productId, correlationId);

    }
}