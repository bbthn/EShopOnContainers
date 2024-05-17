using EventBus.Base.Abstraction;
using Microsoft.Extensions.Logging;
using NotificationService.Api.IntegrationEvents.Events;

namespace NotificationService.Api.IntegrationEvents.EventHandler
{
    public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        private readonly ILogger<OrderCreatedIntegrationEventHandler> _logger;

        public OrderCreatedIntegrationEventHandler(ILogger<OrderCreatedIntegrationEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(OrderCreatedIntegrationEvent @event)
        {
            _logger.LogInformation("NotificationService : Handling integration event: {IntegrationEventId} at BasketSevice.Api",@event.Id);
        }
    }
}
