

using EventBus.Base.Abstraction;
using EventBus.Base.SubManager;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace EventBus.Base.Events
{
    public abstract class BaseEventBus : IEventBus
    {
        private readonly IServiceProvider _serviceProvider;
        public readonly IEventBusSubscriptionManager SubscriptionManager;
        public EventBusConfig EventBusConfig { get; set; }

        public BaseEventBus(EventBusConfig config, IServiceProvider serviceProvider)
        {
            EventBusConfig = config;
            _serviceProvider = serviceProvider;
            SubscriptionManager = new InMemoryEventBusSubscriptionManager(ProcessEventName);       
        }

        public async Task<bool> ProcessEvent(string eventName, string message)
        {
            eventName = ProcessEventName(eventName);
            var processed = false;

            if (SubscriptionManager.HasSubscriptionForEvent(eventName))
            {
                var subscriptions = SubscriptionManager.GetHandlersForEvents(eventName);
                using (var scope = _serviceProvider.CreateScope())
                {
                    foreach (var subscription in subscriptions)
                    {
                        var handler = _serviceProvider.GetService(subscription.HandlerType);
                        if (handler == null) continue;

                        var eventType = SubscriptionManager.GetEventTypeByName($"{EventBusConfig.EventNamePrefix}{eventName}{EventBusConfig.EventNameSuffix}");
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);

                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                    }
                }
                processed = true;               
            }
            return processed;
        }     
        public virtual string ProcessEventName(string eventName)
        {

            if (EventBusConfig.DeleteEventPrefix)
                eventName = eventName.TrimStart(EventBusConfig.EventNamePrefix.ToArray());
            if(EventBusConfig.DeleteEventSuffix)
                eventName = eventName.TrimEnd(EventBusConfig.EventNameSuffix.ToArray());
            return eventName;

        }
        public virtual string GetSubName(string eventName)
        {
            return $"{EventBusConfig.SubscriberClientAppName}.{ProcessEventName(eventName)}";
        }


        public virtual void Dispose()
        {
            EventBusConfig = null;
            this.SubscriptionManager.Clear();
        }
        public abstract void Publish(IntegrationEvent @event);
        public abstract void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
        public abstract void UnSubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
    }
}
