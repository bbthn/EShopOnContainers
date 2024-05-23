using OrderService.Api.Extensions.Registration.EventBust;
using OrderService.Api.Extensions.Registration.EventHandlerRegistration;
using OrderService.Api.Extensions.Registration.ServiceDiscovery;
using OrderService.Application;
using OrderService.Infrastructure;

namespace OrderService.Api.Extensions.Registration
{
    public static class ApplicationRegistration
    {
        public static IServiceCollection ConfigureRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddApplicationRegistration()
                .AddPersistanceRegistration(configuration)
                .AddLogging(opt=>opt.AddConsole())
                .ConfigureAuth(configuration)
                .ConfigureConsul(configuration)
                .ConfigureEventHandler()
                .ConfigureEventBus();         
            return services;
        }
    }
}
