using BasketService.Api.Core.Application.Repository;
using BasketService.Api.IntegrationEvents.Events;
using EventBus.Base.Abstraction;
using StackExchange.Redis;

namespace BasketService.Api.IntegrationEvents.EventHandler
{
    public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        private readonly ILogger<OrderCreatedIntegrationEventHandler> _logger;
        private readonly IBasketRepository _redisbasketRepository;

        public OrderCreatedIntegrationEventHandler(ILogger<OrderCreatedIntegrationEventHandler> logger, IBasketRepository basketRepository)
        {
            _logger = logger;
            _redisbasketRepository = basketRepository;
        }

        public async Task Handle(OrderCreatedIntegrationEvent @event)
        {
            var res = await _redisbasketRepository.DeleteBasketAsync(@event.UserId.ToString());
            _logger.LogInformation("Handling integration event: {IntegrationEventId} at BasketSevice.Api",@event.Id);
        }
    }
}
