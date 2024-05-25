
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NotificationService.Api.IntegrationEvents.EventHandler;
using NotificationService.IntegrationEvents.Handlers;
using RabbitMQ.Client;

namespace NotificationService.ServiceRegistration
{
    public static class AddServices
    {
        public static IServiceCollection AddAppServices(this IServiceCollection service)
        {
            service.AddSingleton(sp =>
            {
                EventBusConfig config = new()
                {
                    ConnectionRetryCount = 5,
                    EventBusType = EventBusType.RabbitMQ,
                    
                    SubscriberClientAppName = "NotificationService",
                    Connection = new ConnectionFactory()
                    {
                        HostName = "s_rabbitmq"
                    }
                };
                return EventBusFactory.Create(config, sp);
            });


            service.AddTransient<OrderPaymentFailedIntegrationEventHandler>();
            service.AddTransient<OrderPaymentSuccessIntegrationEventHandler>();
            service.AddTransient<OrderCreatedIntegrationEventHandler>();
            service.AddLogging(configure => { configure.AddConsole(); });

            return service;

        }


    }
}
