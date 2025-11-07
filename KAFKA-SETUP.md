# Kafka Docker Setup

Bu döküman, Kafka, Zookeeper ve Kafka UI'ın Docker Compose ile nasıl çalıştırılacağını açıklar.

## Gereksinimler

- Docker Desktop yüklü olmalı
- Docker Compose yüklü olmalı

## Servisleri Başlatma

Tüm servisleri başlatmak için:

```bash
docker-compose up -d
```

## Servis Portları

- **Kafka**: `localhost:9092` (Uygulama bağlantısı için)
- **Zookeeper**: `localhost:2181`
- **Kafka UI**: `http://localhost:8080` (Web arayüzü)

## Kafka UI Kullanımı

1. Tarayıcınızda `http://localhost:8080` adresine gidin
2. Topic'leri görüntülemek için "Topics" sekmesine tıklayın
3. `product-events` topic'ini seçerek mesajları görüntüleyin
4. Consumer group'ları ve lag bilgilerini "Consumers" sekmesinden takip edin

## Servisleri Durdurma

```bash
docker-compose down
```

## Logları Görüntüleme

Tüm servislerin loglarını görmek için:

```bash
docker-compose logs -f
```

Sadece Kafka loglarını görmek için:

```bash
docker-compose logs -f kafka
```

## Topic Oluşturma (Manuel)

Kafka otomatik topic oluşturmayı destekler, ancak manuel oluşturmak isterseniz:

```bash
docker exec -it kafka kafka-topics --create --topic product-events --bootstrap-server localhost:9092 --partitions 3 --replication-factor 1
```

## Sorun Giderme

### Kafka bağlantı hatası
- Docker container'larının çalıştığından emin olun: `docker ps`
- Kafka loglarını kontrol edin: `docker-compose logs kafka`

### Port çakışması
- 9092, 2181 veya 8080 portları kullanımda ise docker-compose.yml'deki port mapping'leri değiştirin
