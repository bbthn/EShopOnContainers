

using EventBus.Base.Abstraction;
using MediatR;
using OrderService.Application.IntegrationEvents.Events;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.AggregateModels.OrderAggregate;

namespace OrderService.Application.Features.Commands
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, bool>
    {
        private readonly IOrderRepository orderRepository;
        private readonly IEventBus eventBus;


        public CreateOrderCommandHandler(IOrderRepository orderRepository, IEventBus eventBus)
        {
            this.orderRepository = orderRepository;
            this.eventBus = eventBus;
        }

        public async Task<bool> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {

            var addr = new Address(request.Street, request.City, request.State, request.Country, request.ZipCode);

            Order dbOrder = new(request.UserName,
                addr, request.CardTypeId, request.CardNumber, request.CardSecurityNumber, request.CardHolderName, request.CardExpiration, null);

            foreach (var orderItem in request.OrderItems)
            {
                dbOrder.AddOrderItem(orderItem.ProductId, orderItem.ProductName, orderItem.UnitPrice, orderItem.PictureUrl, orderItem.Units);
            }

            await orderRepository.AddAsync(dbOrder);
            await orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);


            var orderStartedIntegrationEvent = new OrderStartedIntegrationEvent(request.UserName, dbOrder.Id);

            eventBus.Publish(orderStartedIntegrationEvent);


            return true;
        }
    }
}
