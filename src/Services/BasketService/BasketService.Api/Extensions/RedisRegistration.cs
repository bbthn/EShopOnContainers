using StackExchange.Redis;

namespace BasketService.Api.Extensions
{
    public static class RedisRegistration
    {
        public static IServiceCollection ConfigureRedis(this IServiceCollection services,IConfiguration configuration)
        {
            var redisConfig = ConfigurationOptions.Parse(configuration["RedisSettings:ConnectionString"], true);
            redisConfig.ResolveDns = true;
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfig));

            return services;
        }
    }
}
