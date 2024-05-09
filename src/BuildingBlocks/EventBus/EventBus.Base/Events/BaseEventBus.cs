

using EventBus.Base.Abstraction;

namespace EventBus.Base.Events
{
    public abstract class BaseEventBus : IEventBus
    {
        private readonly IServiceProvider _serviceProvider;
        public readonly IEventBusSubscriptionManager SubscriptionManager;
        private EventBusConfig eventBusConfig;

        public BaseEventBus(EventBusConfig eventBusConfig, IServiceProvider serviceProvider)
        {
            
        }
      
        public virtual string ProcessEventName(string eventName)
        {

            if (eventBusConfig.DeleteEventPrefix)
                eventName = eventName.TrimStart(eventBusConfig.EventNamePrefix.ToArray());
            if(eventBusConfig.DeleteEventSuffix)
                eventName = eventName.TrimEnd(eventBusConfig.EventNameSuffix.ToArray());
            return eventName;

        }
        public virtual string GetSubName(string eventName)
        {
            return $"{eventBusConfig.SubscriberClientAppName}.{ProcessEventName(eventName)}";
        }


        public virtual void Dispose()
        {
            eventBusConfig = null;
        }
        public abstract void Publish(IntegrationEvent @event);
        public abstract void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
        public abstract void UnSubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
    }
}
