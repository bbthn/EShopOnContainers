using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OrderService.Infrastructure.Context
{
    public class OrderContextDesignTimeFactory : IDesignTimeDbContextFactory<OrderDbContext>
    {
        private static string ConnectionString = "Data Source=DESKTOP-QSHBLNM\\MSSQLSERVER2022;Initial Catalog=Order;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False";
        public OrderDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<OrderDbContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<OrderDbContext>();
            dbContextOptionsBuilder.UseSqlServer(ConnectionString);
            return new OrderDbContext(dbContextOptionsBuilder.Options, null);
        }      
    }
  
}
