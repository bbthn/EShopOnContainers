
using EventBus.Base.Events;

namespace NotificationService.IntegrationEvents.Events
{
    public class OrderPaymentFailedIntegrationEvent : IntegrationEvent
    {
        public OrderPaymentFailedIntegrationEvent(int orderId, string errMessage)
        {
            OrderId = orderId;
            ErrMessage = errMessage;
        }

        public int OrderId { get; set; }
        public string ErrMessage { get; set; }
    }
}
