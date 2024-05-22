using MediatR;
using OrderService.Domain.SeedWork;
using OrderService.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.Extensions
{
    public static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, OrderDbContext context)
        {
            var domainEntities = context.ChangeTracker
                                        .Entries<BaseEntity>()
                                        .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var @event in domainEvents)
                await mediator.Publish(@event);         
        }
    }
}
