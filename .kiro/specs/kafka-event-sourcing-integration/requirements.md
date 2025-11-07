# Gereksinimler Dökümanı

## Giriş

Bu özellik, mevcut Event Sourcing + CQRS mimarisine Kafka entegrasyonu ekleyerek tam bir event-driven sistem oluşturmayı amaçlar. Yazma işlemleri Kafka üzerinden event'leri yayınlayacak, okuma tarafı ise bu event'leri dinleyerek SQL Server veritabanını güncelleyecektir. Ayrıca Kafka yönetimi için bir UI arayüzü de dahil edilecektir.

## Sözlük

- **Event_Store**: Kafka topic'lerinde saklanan event'lerin koleksiyonu
- **Command_Handler**: MediatR kullanarak command'ları işleyen sınıflar
- **Product_Stream**: Product aggregate'i için event'leri Kafka'ya gönderen stream sınıfı
- **Event_Consumer_Service**: Kafka'dan event'leri dinleyen ve switch-case ile okuma veritabanını güncelleyen BackgroundService
- **Kafka_Producer**: Event'leri Kafka topic'lerine gönderen servis
- **Read_Database**: SQL Server üzerinde tutulan denormalize edilmiş okuma veritabanı
- **Kafka_UI**: Kafka topic'lerini, mesajlarını ve consumer'larını yönetmek için web arayüzü

## Gereksinimler

### Gereksinim 1

**Kullanıcı Hikayesi:** Bir geliştirici olarak, command'lar işlendiğinde ProductStream üzerinden event'lerin Kafka'ya gönderilmesini istiyorum, böylece sistem event-driven mimaride çalışabilsin.

#### Kabul Kriterleri

1. WHEN bir command handler çalıştırıldığında, THE Product_Stream SHALL ilgili event'i Kafka topic'ine gönderir
2. THE Product_Stream SHALL Created, NameChanged, PriceChanged, Deleted metodlarına sahip olur
3. WHILE bir event Kafka'ya gönderilirken, THE Kafka_Producer SHALL event'i JSON formatında serialize eder
4. IF Kafka'ya gönderim başarısız olursa, THEN THE Product_Stream SHALL hata fırlatır ve işlemi geri alır
5. THE Product_Stream SHALL her event için unique bir correlation ID oluşturur

### Gereksinim 2

**Kullanıcı Hikayesi:** Bir sistem yöneticisi olarak, Kafka topic'lerini ve mesajlarını görsel olarak yönetebilmek istiyorum, böylece sistem durumunu kolayca takip edebilim.

#### Kabul Kriterleri

1. THE Kafka_UI SHALL tüm topic'leri listeler
2. WHEN bir topic seçildiğinde, THE Kafka_UI SHALL o topic'teki mesajları gösterir
3. THE Kafka_UI SHALL consumer group'larının durumunu (lag, offset) gösterir
4. THE Kafka_UI SHALL mesaj içeriklerini JSON formatında görüntüler
5. WHERE yönetici yetkisi varsa, THE Kafka_UI SHALL topic oluşturma ve silme imkanı sağlar

### Gereksinim 3

**Kullanıcı Hikayesi:** Bir geliştirici olarak, Kafka'dan gelen event'lerin tek bir BackgroundService ile otomatik olarak okuma veritabanını güncellemesini istiyorum, böylece query'ler güncel veriyi döndürebilsin.

#### Kabul Kriterleri

1. THE Event_Consumer_Service SHALL Kafka topic'lerini sürekli dinler
2. WHEN bir event alındığında, THE Event_Consumer_Service SHALL event'i deserialize eder
3. THE Event_Consumer_Service SHALL switch-case yapısı ile event tipine göre veritabanı güncelleme işlemini yapar
4. THE Event_Consumer_Service SHALL Read_Database'i event verisiyle günceller
5. IF event işleme başarısız olursa, THEN THE Event_Consumer_Service SHALL event'i dead letter queue'ya gönderir

### Gereksinim 4

**Kullanıcı Hikayesi:** Bir geliştirici olarak, sistem başlatıldığında tüm Kafka bileşenlerinin otomatik olarak yapılandırılmasını istiyorum, böylece manuel kurulum gerektirmeden çalışabilsin.

#### Kabul Kriterleri

1. THE System SHALL uygulama başlangıcında Kafka connection'ını test eder
2. THE System SHALL gerekli topic'leri otomatik olarak oluşturur
3. THE System SHALL consumer group'larını yapılandırır
4. THE System SHALL producer ve consumer servislerini dependency injection'a kaydeder
5. WHERE Kafka erişilemezse, THE System SHALL uygun hata mesajı gösterir

### Gereksinim 5

**Kullanıcı Hikayesi:** Bir geliştirici olarak, event'lerin doğru sırada işlenmesini ve veri tutarlılığının korunmasını istiyorum, böylece sistem güvenilir şekilde çalışabilsin.

#### Kabul Kriterleri

1. THE Event_Consumer SHALL event'leri partition key'e göre sıralı işler
2. THE Event_Consumer SHALL duplicate event'leri idempotent şekilde işler
3. THE Event_Consumer SHALL işlenen event'lerin offset'ini commit eder
4. IF bir event işleme sırasında hata oluşursa, THEN THE Event_Consumer SHALL retry mekanizması uygular
5. THE Event_Consumer SHALL maximum retry sayısından sonra event'i dead letter queue'ya gönderir