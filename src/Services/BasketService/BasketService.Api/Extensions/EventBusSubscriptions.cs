using BasketService.Api.IntegrationEvents.EventHandler;
using BasketService.Api.IntegrationEvents.Events;
using EventBus.Base.Abstraction;

namespace BasketService.Api.Extensions
{
    public static class EventBusSubscriptions
    {
        public static IApplicationBuilder AddSubscriptions(this IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
            return app;
        }
    }
}
