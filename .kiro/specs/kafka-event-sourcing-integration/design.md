# Tasarım Dökümanı

## Genel Bakış

Bu tasarım, mevcut Event Sourcing + CQRS mimarisine Kafka entegrasyonu ekler. Yazma tarafında ProductStream ile event'leri Kafka'ya gönderir, okuma tarafında tek BackgroundService ile event'leri dinleyip SQL Server'ı günceller.

## Mimari

### Genel Akış
```
Command → Handler → ProductStream → Kafka Topic → BackgroundService → SQL Server (Read DB)
```

### Bileşenler
1. **Yazma Tarafı (Write Side)**
   - Command Handler'lar (mevcut - güncellenecek)
   - ProductStream (yeni)
   - Kafka Producer Service (yeni)

2. **Okuma Tarafı (Read Side)**
   - Kafka Consumer BackgroundService (yeni)
   - SQL Server Read Database (mevcut)

## Bileşenler ve Arayüzler

### 1. ProductStream Sınıfı
```csharp
public class ProductStream
{
    private readonly IKafkaProducer _kafkaProducer;
    
    public async Task Created(CreateProductDto dto)
    public async Task NameChanged(ChangeProductNameDto dto)  
    public async Task PriceChanged(ChangeProductPriceDto dto)
    public async Task Deleted(Guid productId)
}
```

**Sorumluluklar:**
- DTO'ları event'lere dönüştürme
- Event'leri Kafka topic'ine gönderme

### 2. Kafka Producer Service
```csharp
public interface IKafkaProducer
{
    Task PublishAsync<T>(string topic, T message, string key = null);
}

public class KafkaProducer : IKafkaProducer
{
    // Confluent.Kafka kullanarak implementation
}
```

**Sorumluluklar:**
- Event'leri JSON serialize etme
- Kafka'ya mesaj gönderme
- Connection yönetimi

### 3. Event Consumer BackgroundService
```csharp
public class EventConsumerService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Kafka consumer loop
        // Switch-case ile event type'a göre DB güncelleme
    }
    
    private async Task HandleEvent(string eventType, string eventData)
    {
        switch (eventType)
        {
            case nameof(ProductCreatedEvent):
                // Product oluştur
                break;
            case nameof(ProductNameChangedEvent):
                // Product name güncelle
                break;
            // diğer case'ler...
        }
    }
}
```

**Sorumluluklar:**
- Kafka topic'lerini sürekli dinleme
- Event'leri deserialize etme
- Switch-case ile event tipine göre DB güncelleme
- Hata yönetimi ve retry mekanizması

### 4. Command Handler Güncellemeleri
Mevcut handler'lar ProductStream'i kullanacak şekilde güncellenecek:

```csharp
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand>
{
    private readonly ProductStream _productStream;
    
    public async Task<Unit> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        await _productStream.Created(request.CreateProductDto);
        return Unit.Value;
    }
}
```

## Veri Modelleri

### Kafka Configuration
```csharp
public class KafkaSettings
{
    public string BootstrapServers { get; set; }
    public string GroupId { get; set; }
    public string ProductTopic { get; set; }
}
```

## Kafka UI

### Özellikler
- Docker container olarak çalışacak (örn: provectuslabs/kafka-ui)
- Topic mesajlarını görüntüleme
- JSON formatında event içeriği
- Kafka cluster yönetimi
- Docker Compose ile birlikte yapılandırılacak

