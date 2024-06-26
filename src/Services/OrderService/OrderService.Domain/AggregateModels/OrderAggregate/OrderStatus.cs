﻿using OrderService.Domain.Exceptions;
using OrderService.Domain.SeedWork;

namespace OrderService.Domain.AggregateModels.OrderAggregate
{
    public class OrderStatus : Enumaration
    {
        public static OrderStatus Submitted = new OrderStatus(1, nameof(Submitted).ToLowerInvariant());
        public static OrderStatus AwaitingValidation = new OrderStatus(2, nameof(AwaitingValidation).ToLowerInvariant());
        public static OrderStatus StockConfirmed = new OrderStatus(3, nameof(StockConfirmed).ToLowerInvariant());
        public static OrderStatus Paid = new OrderStatus(4, nameof(Paid).ToLowerInvariant());
        public static OrderStatus Shipped = new OrderStatus(5, nameof(Shipped).ToLowerInvariant());
        public static OrderStatus Cancelled = new OrderStatus(6, nameof(Cancelled).ToLowerInvariant());

        public OrderStatus(int id, string name) : base(id, name) { }       
        public static IEnumerable<OrderStatus> List() => new[] { Submitted, AwaitingValidation, StockConfirmed, Paid , Shipped, Cancelled};

        public  OrderStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            return state ?? throw new OrderingDomainException($"Possible values for OrderStatus : {String.Join(',', List().Select(s => s.Name))}");
        }

        public static OrderStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);
            return state ?? throw new OrderingDomainException($"Possible values for OrderStatus : {String.Join(',', List().Select(s => s.Name))}");
        }


    }
}
