
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NotificationService.IntegrationEvents.Handlers;

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
                    
                    SubscriberClientAppName = "NotificationService"
                };
                return EventBusFactory.Create(config, sp);
            });

            service.AddTransient<OrderPaymentFailedIntegrationEventHandler>();
            service.AddTransient<OrderPaymentSuccessIntegrationEventHandler>();

            service.AddLogging(configure => { configure.AddConsole(); });

            return service;

        }


    }
}
