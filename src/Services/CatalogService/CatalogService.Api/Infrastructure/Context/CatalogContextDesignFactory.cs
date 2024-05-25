using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CatalogService.Api.Infrastructure.Context
{
    public class CatalogContextDesignFactory : IDesignTimeDbContextFactory<CatalogContext>
    {
        private static string ConnectionString = "Data Source=s_sqlserver;Initial Catalog=EShopOnContainers;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False";
        public CatalogContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<CatalogContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<CatalogContext>();
            dbContextOptionsBuilder.UseSqlServer(ConnectionString);
            return new CatalogContext(dbContextOptionsBuilder.Options);          
        }
    }
}
