Merhaba, bu proje, güçlü bir mikroservis ekosisteminin nasıl kurulabileceğini göstermektedir. Projemizde altı farklı servis ve bir API Gateway bulunmaktadır.
Genel bakış:
Mikroservisler:
NotificationService
PaymentService
OrderService
BasketService
CatalogService
IdentityService

API Gateway:
ApiGateway, dinamik servis keşfi ve sağlık kontrolleri için Ocelot ve Consul kullanılarak yapılandırılmıştır.

Proje Özellikleri:

BasketService:
Basket bilgileri Redis üzerinde kullanıcı adı key olacak şekilde tutularak bir yapı oluşturuldu.

OrderService:
CQRS ve MediatR desenleri kullanılarak, sorumlulukların temiz bir şekilde ayrılması ve ölçeklenebilirliğin artırılması sağlanmıştır.

Diğer Servisler:
Sadelik İçin Basit İmplementasyon: NotificationService, PaymentService, BasketService, CatalogService ve IdentityService, basit ve anlaşılır bir yapıyla implemente edilmiştir.

EventBus:
Message Brokers: Güvenilir event-driven iletişim için RabbitMQ ve Azure Service Bus ile entegrasyon sağlanmıştır.
InMemory Event Handling: Event-EventHandler çiftleri bellek içinde yönetilerek hızlı ve verimli olay işleme sağlanmıştır. Gerektiğinde veritabanı ile ilerlenebilir.

API Gateway:
Ocelot Integration: Ocelot, gelen istekleri ilgili mikroservislere yönlendirir.
Consul Integration: Servisler, başlatıldıklarında kendilerini Consul'a kaydeder. Bu yapı, dinamik servis keşfi ve sürekli health check'ler ile dayanıklı ve hata toleranslı bir yapı sağlar.

Güvenlik:
Authorization: OrderService ve BasketService için authentication protokollerinden yararlanılarak  yetkilendirme mekanizması kullanılmıştır.
