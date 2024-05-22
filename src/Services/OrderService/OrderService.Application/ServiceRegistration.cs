using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace OrderService.Application
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationRegistration(this IServiceCollection services)
        {
            var assm = Assembly.GetExecutingAssembly();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assm));
            services.AddAutoMapper(assm);

            return services;
        }
    }
}
