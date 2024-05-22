

using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Domain.Models;
using OrderService.Domain.SeedWork;
using OrderService.Infrastructure.EntityConfigurations;
using OrderService.Infrastructure.Extensions;

namespace OrderService.Infrastructure.Context
{
    public class OrderDbContext : DbContext , IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "ordering";
        private readonly IMediator mediator;
        public OrderDbContext(DbContextOptions dbContextOptions,IMediator mediator):base(dbContextOptions)
        {
            this.mediator = mediator;
        }
        public OrderDbContext():base()
        {
            
        }

        DbSet<CardType> CardTypes { get; set; }
        DbSet<PaymentMethod> PaymentMethods { get; set; }
        DbSet<Buyer> Buyers { get; set; }
        DbSet<Order> Orders { get; set; }
        DbSet<OrderItem> OrderItems { get; set; }
        DbSet<OrderStatus> OrderStatus { get; set; }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await mediator.DispatchDomainEventsAsync(this);
            await base.SaveChangesAsync(cancellationToken);
            return true;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OrderEntityConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemEntityConfiguration());
            modelBuilder.ApplyConfiguration(new BuyerEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderStatusEntityConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentMethodEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CardTypeEntityConfiguration());

        }


    }
}
