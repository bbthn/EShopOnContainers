
using EventBus.Base.Events;

namespace EventBus.xUnitTests.Events.Events
{
    public class OrderCreatedIntegrationEvent : IntegrationEvent
    {
        public OrderCreatedIntegrationEvent(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
}
