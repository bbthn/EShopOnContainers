
using EventBus.Base.Events;

namespace EventBus.Base.Abstraction
{
    public interface IEventBusSubscriptionManager
    {
        bool IsEmpty { get; }
        event EventHandler<string> onEventRemoved;
        void AddSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
        void RemoveSubscription<T,TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
        bool HasSubscriptionForEvent<T>() where T: IntegrationEvent;
        bool HasSubscriptionForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        void Clear();
        IEnumerable<SubscriptionInfo> GetHandlersForEvents<T>() where T : IntegrationEvent;
        IEnumerable<SubscriptionInfo> GetHandlersForEvents(string eventName);
        string GetEventKey<T>();


    }
}
