using EventBus.Base.Events;

namespace PaymentService.Api.IntegrationEvents.Events
{
    public class OrderPaymentFailedIntegrationEvent : IntegrationEvent
    {
        public OrderPaymentFailedIntegrationEvent(int orderId, string errMessage)
        {
            OrderId = orderId;
            ErrMessage = errMessage;
        }
        public int OrderId { get; }
        public string ErrMessage { get;}

    }
}
