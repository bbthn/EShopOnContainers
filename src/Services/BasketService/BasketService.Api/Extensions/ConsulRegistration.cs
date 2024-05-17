using Consul;


namespace BasketService.Api.Infrastructure.Extensions
{
    public static class ConsulRegistration
    {
        public static IServiceCollection ConfigureConsul(this IServiceCollection services,
            IConfiguration configuration)
        {
            var config = new ConsulClientConfiguration() { Address = new Uri(configuration["ConsulConfig:Address"]) };
            services.AddSingleton<IConsulClient>(consulClient =>
            {
                return new ConsulClient(config);
            });

            return services;

        }

        public static IApplicationBuilder RegisterWithConsul(this IApplicationBuilder app,IHostApplicationLifetime lifetime,IConfiguration configuration)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();  
            var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

            var logger = loggingFactory.CreateLogger<IApplicationBuilder>();

            //get server IP address
            var uri = configuration.GetValue<Uri>("ConsulConfig:ServiceAddress");
            var serviceName = configuration.GetValue<string>("ConsulConfig:ServiceName");
            var serviceId = configuration.GetValue<string>("ConsulConfig:ServiceId");

            //register service with consul
           
            var registration = new AgentServiceRegistration()
            {
                ID =  serviceId ?? $"BasketService",
                Name = serviceName ?? "BasketService",
                Address = $"{uri.Host}",
                Port = uri.Port,
                Tags = new[] { "Basket Service", "Basket" }
            };

            logger.LogInformation("Registering with Consul");
            consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            consulClient.Agent.ServiceRegister(registration).Wait();

            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Deregistering from Consul");
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });

            return app;
        }
    }
}
