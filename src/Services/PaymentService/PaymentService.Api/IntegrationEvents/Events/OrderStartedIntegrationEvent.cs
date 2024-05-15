
using EventBus.Base.Events;

namespace PaymentService.Api.IntegrationEvents.Events
{
    public class OrderStartedIntegrationEvent : IntegrationEvent
    {
        public OrderStartedIntegrationEvent(int orderId)
        {
            OrderId = orderId;
        }
        public OrderStartedIntegrationEvent()
        {}
        public int OrderId { get; set; }
    }
}
