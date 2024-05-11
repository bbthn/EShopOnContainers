
using EventBus.Base.Abstraction;
using EventBus.xUnitTests.Events.Events;

namespace EventBus.xUnitTests.Events.EventHandlers
{
    public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        public Task Handle(OrderCreatedIntegrationEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}
