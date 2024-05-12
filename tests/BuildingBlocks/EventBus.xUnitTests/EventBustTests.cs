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
        public void subscribe_event_on_rabbitmq_test()
        {
            this.services.AddSingleton<IEventBus>(sp =>
            {
                return EventBusFactory.Create(GetRabbitMQBus(), sp);
            });
            var sp = this.services.BuildServiceProvider();
            var eventBus = sp.GetService<IEventBus>();

            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

            //eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
        }
        [Fact]
        public void subscribe_event_on_azureservicebus_test()
        {
            this.services.AddSingleton<IEventBus>(sp =>
            {
                return EventBusFactory.Create(GetAzureBus(), sp);
            });
            var sp = this.services.BuildServiceProvider();
            var eventBus = sp.GetService<IEventBus>();
            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
            //eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
            Task.Delay(2000).Wait();
        }
        [Fact]
        public void send_message_to_rabbitmqbus()
        {
            this.services.AddSingleton<IEventBus>(sp =>
            {
                return EventBusFactory.Create(GetRabbitMQBus(), sp);
            });
            var sp = this.services.BuildServiceProvider();
            var eventBus = sp.GetService<IEventBus>();
            eventBus.Publish(new OrderCreatedIntegrationEvent(1));

        }

        [Fact]
        public void send_message_to_azurebus()
        {
            this.services.AddSingleton<IEventBus>(sp =>
            {
                return EventBusFactory.Create(GetAzureBus(), sp);
            });
            var sp = this.services.BuildServiceProvider();
            var eventBus = sp.GetService<IEventBus>();
            eventBus.Publish(new OrderCreatedIntegrationEvent(1));

        }



 


        private EventBusConfig GetAzureBus()
        {
            return new EventBusConfig()
            {
                ConnectionRetryCount = 5,
                SubscriberClientAppName = "EventBus.UnitTest",
                DefaultTopicName = "BBTopicName", //exchangeName
                EventBusType = EventBusType.AzureServiceBus,
                EventNameSuffix = "IntegrationEvent",
                EventBustConnectionString = "Endpoint=sb://bbapplication.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=CwWzlyTWtGZg64YgweXdVtTL8bGJD5B7f+ASbMuT3Ko="
            };
        }
        private EventBusConfig GetRabbitMQBus()
        {
            return new EventBusConfig()
            {
                ConnectionRetryCount = 5,
                SubscriberClientAppName = "EventBus.UnitTest",
                DefaultTopicName = "BBTopicName", //exchangeName
                EventBusType = EventBusType.RabbitMQ,
                EventNameSuffix = "IntegrationEvent"
            };
        }
    }
}