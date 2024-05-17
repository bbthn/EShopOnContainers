using BasketService.Api.Core.Application.Repository;
using BasketService.Api.Core.Application.Services;
using BasketService.Api.Infrastructure.Extensions;
using BasketService.Api.Infrastructure.Repository;

namespace BasketService.Api.Extensions
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AppServiceRegister(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureConsul(configuration);
            services.AuthConfigure(configuration);
            services.ConfigureRedis(configuration);
            services.EventBusConfiguration();
            services.AddHttpContextAccessor();

            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IBasketRepository, BasketRepository>();
            
            return services;
        }

    }
}
