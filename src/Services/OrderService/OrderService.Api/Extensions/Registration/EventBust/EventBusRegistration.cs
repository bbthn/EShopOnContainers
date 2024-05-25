using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using RabbitMQ.Client;

namespace OrderService.Api.Extensions.Registration.EventBust
{
    public static class EventBusRegistration
    {
        public static IServiceCollection ConfigureEventBus(this IServiceCollection services)
        {
            services.AddSingleton<IEventBus>(sp =>
            {
                EventBusConfig config = new()
                {
                    ConnectionRetryCount = 5,
                    EventBusType = EventBusType.RabbitMQ,
                    SubscriberClientAppName = "OrderService",
                    Connection = new ConnectionFactory()
                    {
                        HostName = "s_rabbitmq"
                    }
                };

                return EventBusFactory.Create(config, sp);
            });
            return services;

        }
    }
}
