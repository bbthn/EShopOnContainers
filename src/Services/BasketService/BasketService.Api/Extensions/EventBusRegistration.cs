using BasketService.Api.IntegrationEvents.EventHandler;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;

namespace BasketService.Api.Extensions
{
    public static class EventBusRegistration
    {
        public static IServiceCollection EventBusConfiguration(this IServiceCollection services)
        {
            services.AddSingleton<IEventBus>(sp =>
            {
                EventBusConfig conf = new()
                {
                    ConnectionRetryCount = 5,
                    EventBusType = EventBusType.RabbitMQ,
                    SubscriberClientAppName = "BasketService"
                };

                return EventBusFactory.Create(conf, sp);
            });

            services.AddTransient<OrderCreatedIntegrationEventHandler>();
            return services;
        }

       
    }
}
