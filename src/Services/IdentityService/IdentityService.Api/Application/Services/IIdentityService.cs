using IdentityService.Api.Application.Models;
using Microsoft.Extensions.Configuration;

namespace IdentityService.Api.Application.Services
{
    public interface IIdentityService
    {
        Task<LoginResponseModel> Login(LoginRequestModel loginRequestModel);
    }
}
