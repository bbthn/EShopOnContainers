using BasketService.Api.Core.Application.Repository;
using BasketService.Api.Core.Domain.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace BasketService.Api.Infrastructure.Repository
{
    public class BasketRepository : IBasketRepository
    {
        private readonly ILogger<BasketRepository> _logger;
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public BasketRepository(ILogger<BasketRepository> logger, IConnectionMultiplexer redis)
        {
            _logger = logger;
            _redis = redis;
            _database = _redis.GetDatabase();
        }

        public async Task<bool> DeleteBasketAsync(string id)
        {
            return await _database.KeyDeleteAsync(id);
        }
        public IEnumerable<string> GetUsers()
        {
            IServer server =  this.GetServer();
            var data = server.Keys();

            return data?.Select(k => k.ToString());
        }

        public async Task<CustomerBasket> GetBasketAsync(string customerId)
        {
            var basket= await _database.StringGetAsync(customerId);
            if (basket.IsNullOrEmpty)
                return null;
            return JsonConvert.DeserializeObject<CustomerBasket>(basket);
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            var seriliazedBasket = JsonConvert.SerializeObject(basket);
            var created = await _database.StringSetAsync(basket.BuyerId, seriliazedBasket);
            if (!created)
            {
                _logger.LogInformation("Problem occur persisting the item");
                return null;
            }
            _logger.LogInformation("Basket item persisted successfully!");
            return await this.GetBasketAsync(basket.BuyerId);
        }

        private IServer GetServer()
        {
            var endpoints = _redis.GetEndPoints();
            return _redis.GetServer(endpoints.First());
        }
    }
}
