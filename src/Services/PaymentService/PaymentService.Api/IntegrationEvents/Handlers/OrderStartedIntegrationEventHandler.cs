﻿
using EventBus.Base.Abstraction;
using EventBus.Base.Events;
using PaymentService.Api.IntegrationEvents.Events;

namespace PaymentService.Api.IntegrationEvents.Handlers
{
    public class OrderStartedIntegrationEventHandler : IIntegrationEventHandler<OrderStartedIntegrationEvent>
    {
        private readonly IConfiguration _configuration;
        private readonly IEventBus _eventBus;
        private readonly ILogger<OrderStartedIntegrationEventHandler> _logger;

        public OrderStartedIntegrationEventHandler(IConfiguration configuration, IEventBus eventBus, ILogger<OrderStartedIntegrationEventHandler> logger)
        {
            _configuration = configuration;
            _eventBus = eventBus;
            _logger = logger;
        }

        public Task Handle(OrderStartedIntegrationEvent @event)
        {
            // Fake Payment Process
            string keyword = "PaymentSuccess";
            bool paymentSuccessFlag = _configuration.GetValue<bool>(keyword);

            IntegrationEvent paymentEvent = paymentSuccessFlag
                ? new OrderPaymentSuccessIntegrationEvent(@event.OrderId)
                : new OrderPaymentFailedIntegrationEvent(@event.OrderId,"Operation has failed!");

            _logger.LogInformation($"OrderCreatedIntegrationEventHandler in PaymentService is fired with PaymentService: {paymentSuccessFlag}, orderId:{@event.OrderId}");      
            _eventBus.Publish(paymentEvent);
            return Task.CompletedTask;   
        }
    }
}
