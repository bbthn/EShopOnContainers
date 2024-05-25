using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using PaymentService.Api.IntegrationEvents.Handlers;
using RabbitMQ.Client;

namespace PaymentService.Api.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddLogging(configure => { configure.AddConsole(); });

            services.AddTransient<OrderStartedIntegrationEventHandler>();

            services.AddSingleton<IEventBus>(sp =>
            {
                EventBusConfig config = new()
                {
                    ConnectionRetryCount = 5,
                    EventNameSuffix = "IntegrationEvent",
                    SubscriberClientAppName = "PaymentService",
                    EventBusType=EventBusType.RabbitMQ,
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
