using Consul;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;

namespace IdentityService.Api.Application
{
    public static class ConsulConfigurations
    {
        public static IServiceCollection ConfigureConsul(this IServiceCollection services,
            IConfiguration configuration)
        {
            var config = new ConsulClientConfiguration() { Address = new Uri(configuration.GetSection("ConsulConfig").GetValue<string>("Address")) };
            services.AddSingleton<IConsulClient>(consulClient =>
            {
                return new ConsulClient(config);
            });

            return services;

        }

        public static IApplicationBuilder RegisterWithConsul(this IApplicationBuilder app, IHostApplicationLifetime lifetime, IConfiguration configuration)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

            var logger = loggingFactory.CreateLogger<IApplicationBuilder>();

            //get server IP address
            //get server IP address
            var uri = configuration.GetValue<Uri>("ConsulConfig:ServiceAddress");
            var serviceName = configuration.GetValue<string>("ConsulConfig:ServiceName");
            var serviceId = configuration.GetValue<string>("ConsulConfig:ServiceId");

            //register service with consul
            var registration = new AgentServiceRegistration()
            {
                ID = serviceId ?? $"IdentityService",
                Name = serviceName ??  "IdentityService",
                Address = $"{uri.Host}",
                Port = uri.Port,
                Tags = new[] { "Identity Service", "Identity" }
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

