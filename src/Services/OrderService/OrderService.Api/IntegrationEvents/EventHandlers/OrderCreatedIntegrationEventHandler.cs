﻿using EventBus.Base.Abstraction;
using MediatR;
using OrderService.Api.IntegrationEvents.Events;
using OrderService.Application.Features.Commands;

namespace OrderService.Api.IntegrationEvents.EventHandlers
{
    public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        private readonly IMediator mediator;
        private readonly ILogger<OrderCreatedIntegrationEventHandler> logger;

        public OrderCreatedIntegrationEventHandler(IMediator mediator, ILogger<OrderCreatedIntegrationEventHandler> logger)
        {
            this.mediator = mediator;
            this.logger = logger;
        }
        public async Task Handle(OrderCreatedIntegrationEvent @event)
        {
            try
            {
                logger.LogInformation("Handling integration event: {IntegrationEventId} at 'OrderService.Api' - ({@IntegrationEvent})",
                @event.Id,
                @event);

                var createOrderCommand = new CreateOrderCommand(@event.Basket.Items,
                                @event.UserId, @event.UserName,
                                @event.City, @event.Street,
                                @event.State, @event.Country, @event.ZipCode,
                                @event.CardNumber, @event.CardHolderName, @event.CardExpiration,
                                @event.CardSecurityNumber, @event.CardTypeId);

                await mediator.Send(createOrderCommand);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }
        }
    }
}
