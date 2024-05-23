using EventBus.Base.Abstraction;
using OrderService.Api.IntegrationEvents.EventHandlers;
using OrderService.Api.IntegrationEvents.Events;

namespace OrderService.Api.Extensions
{
    public static class AppBuilderExtension
    {
        public static IApplicationBuilder ConfigureSubscription(this IApplicationBuilder app)
        {
            var eventbus= app.ApplicationServices.GetService<IEventBus>();
            eventbus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

            return app;
        }
    }
}
