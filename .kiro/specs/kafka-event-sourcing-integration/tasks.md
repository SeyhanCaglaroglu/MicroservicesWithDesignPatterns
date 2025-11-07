# Uygulama Planı

- [x] 1. Kafka bağımlılıklarını ve yapılandırmasını kurma

  - EventSourcing.API projesine Confluent.Kafka NuGet paketini ekleme
  - appsettings.json'a Kafka yapılandırması ekleme
  - KafkaSettings sınıfını oluşturma
  - _Gereksinimler: 4.2, 4.4_

- [x] 2. Kafka Producer servisini implement etme

  - IKafkaProducer interface'ini oluşturma
  - KafkaProducer sınıfını implement etme
  - JSON serialization desteği ekleme
  - Dependency injection'a kaydetme
  - _Gereksinimler: 1.2, 1.3_

- [x] 3. ProductStream sınıfını oluşturma

  - ProductStream sınıfını oluşturma
  - Created, NameChanged, PriceChanged, Deleted metodlarını implement etme
  - DTO'ları event'lere dönüştürme logic'i ekleme
  - Kafka Producer'ı kullanarak event'leri gönderme
  - _Gereksinimler: 1.1, 1.4, 1.5_

- [x] 4. Command Handler'ları güncelleme

  - CreateProductCommandHandler'ı ProductStream kullanacak şekilde güncelleme
  - ChangeProductNameCommandHandler'ı ProductStream kullanacak şekilde güncelleme
  - ChangeProductPriceCommandHandler'ı ProductStream kullanacak şekilde güncelleme
  - DeleteProductCommandHandler'ı ProductStream kullanacak şekilde güncelleme
  - _Gereksinimler: 1.1_

- [x] 5. Event Consumer BackgroundService oluşturma

  - EventConsumerService BackgroundService sınıfını oluşturma
  - Kafka consumer yapılandırması ekleme
  - Event'leri sürekli dinleme loop'u implement etme
  - Event deserialization logic'i ekleme
  - _Gereksinimler: 3.1, 3.2_

- [ ] 6. Event handling switch-case logic'i implement etme
  - HandleEvent metodunu oluşturma
  - ProductCreatedEvent için veritabanı güncelleme logic'i
  - ProductNameChangedEvent için veritabanı güncelleme logic'i
  - ProductPriceChangedEvent için veritabanı güncelleme logic'i
  - ProductDeletedEvent için veritabanı güncelleme logic'i
  - _Gereksinimler: 3.3, 3.4_

- [x] 7. Docker Compose ile Kafka ve UI kurulumu

  - docker-compose.yml dosyası oluşturma
  - Kafka ve Zookeeper servislerini yapılandırma
  - Kafka UI container'ını ekleme
  - Network ve port yapılandırması
  - _Gereksinimler: 2.1, 2.2, 2.3, 2.4_

- [x] 8. Sistem entegrasyonu ve test





  - EventConsumerService'i Program.cs'e kaydetme
  - Kafka bağlantı testleri
  - End-to-end event flow testi (command → event → database güncelleme)
  - _Gereksinimler: 4.1, 4.3, 4.5_

- [ ] 9. Hata yönetimi ve retry mekanizması
  - Producer için hata yönetimi ekleme
  - Consumer için retry mekanizması implement etme
  - Dead letter queue logic'i ekleme
  - _Gereksinimler: 3.5, 5.4, 5.5_

- [ ] 10. İdempotency ve sıralama
  - Event'lerin idempotent işlenmesi
  - Partition key ile sıralı işleme
  - Duplicate event kontrolü
  - _Gereksinimler: 5.1, 5.2, 5.3_