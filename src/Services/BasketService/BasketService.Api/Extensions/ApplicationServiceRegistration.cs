using BasketService.Api.Core.Application.Repository;
using BasketService.Api.Core.Application.Services;
using BasketService.Api.Infrastructure.Extensions;
using BasketService.Api.Infrastructure.Repository;
using Serilog;

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
            services.ConfigureSerilog();

            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IBasketRepository, BasketRepository>();
            
            return services;
        }

        public static void ConfigureSerilog(this IServiceCollection services)
        {
            Serilog.Core.Logger logger = new LoggerConfiguration()
            .ReadFrom.Configuration(ConfigurationSetting.serilogConfiguration)
            .CreateLogger();

            services.AddLogging(x =>
            {
                x.AddSerilog(logger);
            });
        }

    }
}
