using BasketService.Api.Core.Application.Repository;
using BasketService.Api.Core.Application.Services;
using BasketService.Api.Core.Domain.Models;
using BasketService.Api.IntegrationEvents.Events;
using EventBus.Base.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BasketService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _redisBasketRepository;
        private readonly IIdentityService _identityService;
        private IEventBus _eventBus;
        private readonly ILogger<BasketController> _logger;

        public BasketController(IBasketRepository redisBasketRepository, IIdentityService identityService, IEventBus eventBus, ILogger<BasketController> logger)
        {
            _redisBasketRepository = redisBasketRepository;
            _identityService = identityService;
            _eventBus = eventBus;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Basket Service is Up and Running");
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> GetBasketByIdAsync(string id)
        {
            var basket = await _redisBasketRepository.GetBasketAsync(id);
            return Ok(basket ?? new CustomerBasket(id));
        }

        [HttpPut]
        [Route("update")]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> UpdateBasketByIdAsync([FromBody] CustomerBasket basket)
        {
            return Ok(await _redisBasketRepository.UpdateBasketAsync(basket));
        }

        [HttpPost("additem")]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> AddItemToBasketAsync([FromBody] BasketItem basketItem)
        {
            var userId = _identityService.GetUserName().ToString();
            var basket = await _redisBasketRepository.GetBasketAsync(userId);

            if (basket == null)
                basket = new CustomerBasket(userId);

            basket.Items.Add(basketItem);

            basket = await _redisBasketRepository.UpdateBasketAsync(basket);
            return Ok(basket);
        }
        //delete api/basket/5
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task DeleteBasketByIdAsync(string id)
        {
            _ = await _redisBasketRepository.DeleteBasketAsync(id);

        }

        [Route("checkout")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CheckoutAsync([FromBody] BasketCheckout basketCheckout)
        {
            var userId = basketCheckout.Buyer;

            var basket = await _redisBasketRepository.GetBasketAsync(userId);

            if (basket == null)
                return BadRequest();

            var userName = _identityService.GetUserName();

            var eventMessage = new OrderCreatedIntegrationEvent(userId, userName, basketCheckout.City, basketCheckout.Street,
               basketCheckout.State, basketCheckout.Country, basketCheckout.ZipCode, basketCheckout.CardNumber, basketCheckout.CardHolderName,
               basketCheckout.CardExpiration, basketCheckout.CardSecurityNumber, basketCheckout.CardTypeId, basketCheckout.Buyer, basket);

            try
            {
                _eventBus.Publish(eventMessage);

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "ERROR Publishing integration event: {IntegrationEventId} from {BasketService.App}", eventMessage.Id);
                throw;
            }
            return Accepted();
        }
    }
}
