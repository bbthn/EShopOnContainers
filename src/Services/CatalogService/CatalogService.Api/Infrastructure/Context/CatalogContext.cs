using CatalogService.Api.Core.Domain;
using CatalogService.Api.Infrastructure.EntityConfiguration;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CatalogService.Api.Infrastructure.Context
{
    public class CatalogContext : DbContext
    {
        public const string DEFAULT_SCHEMA = "catalog";
        public CatalogContext(DbContextOptions<CatalogContext> contextOptions) : base(contextOptions)
        { }
        DbSet<CatalogBrand> Brands { get; set; }
        DbSet<CatalogItem> Items { get; set; }
        DbSet<CatalogType> CatalogTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CatalogBrandEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CatalogItemEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CatalogTypeEntityConfiguration());
            

            base.OnModelCreating(modelBuilder);
        }

    }
}
