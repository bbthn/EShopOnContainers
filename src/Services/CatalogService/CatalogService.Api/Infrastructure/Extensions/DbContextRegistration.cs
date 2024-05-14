using CatalogService.Api.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CatalogService.Api.Infrastructure.Extensions
{
    public static class DbContextRegistration
    {
        public static IServiceCollection AddDbContextService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CatalogContext>(optionsAction =>
            {
                optionsAction.UseSqlServer(configuration.GetConnectionString("localdb"),
                    sqlopt => {
                        sqlopt.EnableRetryOnFailure(maxRetryCount: 15);
                        sqlopt.MigrationsAssembly(typeof(Program)
                            .GetTypeInfo().Assembly.GetName().Name);
                                });
      
            });


            return services;
        }
    }
}
