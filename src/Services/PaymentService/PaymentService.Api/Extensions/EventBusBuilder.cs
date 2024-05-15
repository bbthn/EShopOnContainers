using EventBus.Base.Abstraction;
using PaymentService.Api.IntegrationEvents.Events;
using PaymentService.Api.IntegrationEvents.Handlers;

namespace PaymentService.Api.Extensions
{
    public static class EventBusBuilder
    {

        public static IApplicationBuilder UseEventBus(this IApplicationBuilder app)
        {
            IEventBus eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>();
            return app;
        }
    }
}
