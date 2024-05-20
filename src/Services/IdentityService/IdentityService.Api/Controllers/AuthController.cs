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
        private readonly ILogger<AuthController> _logger;

        public AuthController(IIdentityService identityService, ILogger<AuthController> logger)
        {
            _identityService = identityService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(LoginResponseModel),(int)HttpStatusCode.OK)]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel loginRequestModel)
        {
            if (loginRequestModel == null)

                return BadRequest();
            var response =  _identityService.Login(loginRequestModel);
            _logger.LogInformation("Welcome {usrname}", response.Result.UserName);
            if(response == null)
                return BadRequest();
            return Ok(response);
        }
    }
}
