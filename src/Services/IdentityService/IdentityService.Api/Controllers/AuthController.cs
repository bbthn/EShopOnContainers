using IdentityService.Api.Application.Models;
using IdentityService.Api.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace IdentityService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(LoginResponseModel),(int)HttpStatusCode.OK)]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel loginRequestModel)
        {
            if (loginRequestModel == null)
                return BadRequest();
            var response =  _identityService.Login(loginRequestModel);
            if(response == null)
                return BadRequest();
            return Ok(response);
        }
    }
}
