using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.xUnitTests.Events.EventHandlers;
using EventBus.xUnitTests.Events.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventBus.xUnitTests
{
    public class EventBustTests : IClassFixture<ServiceCollection>
    {
        
        private ServiceCollection services;

        public EventBustTests(ServiceCollection services)
        {
            this.services = new ServiceCollection();
            this.services.AddLogging(configure => configure.AddConsole());
        }

        [Fact]
        public void Subscribe_event_on_rabbitmq_test()
        {
            this.services.AddSingleton<IEventBus>(sp =>
            {
                EventBusConfig config = new EventBusConfig()
                {
                    ConnectionRetryCount = 5,
                    SubscriberClientAppName = "EventBus.UnitTest",
                    DefaultTopicName = "BBTopicName", //exchangeName
                    EventBusType = EventBusType.RabbitMQ,
                    EventNameSuffix = "IntegrationEvent",
                    //Connection = new ConnectionFactory default settings
                    //{
                    //    HostName="localhost",
                    //    Port=5672,
                    //    UserName="guest",
                    //    Password="password"
                    //}
                };

                return EventBusFactory.Create(config, sp);
            });


            var sp = this.services.BuildServiceProvider();

            var eventBus = sp.GetService<IEventBus>();

            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

            eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
        }
    }
}