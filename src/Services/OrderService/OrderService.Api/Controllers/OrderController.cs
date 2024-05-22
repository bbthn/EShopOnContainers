using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Features.Commands;
using OrderService.Application.Features.Queries.OrderQueries;

namespace OrderService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMediator mediator;

        public OrderController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDetailsById(Guid id)
        {
            var res = await mediator.Send(new GetOrderDetailsQuery(id));
            return Ok(res);
        }
    }
}
