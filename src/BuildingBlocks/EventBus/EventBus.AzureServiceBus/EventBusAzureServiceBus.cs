using EventBus.Base;
using EventBus.Base.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace EventBus.AzureServiceBus
{
    public class EventBusAzureServiceBus : BaseEventBus
    {
        private ITopicClient _topicClient;
        private ManagementClient _managementClient;
        private ILogger _logger;
        public EventBusAzureServiceBus(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
        {
            this._managementClient = new ManagementClient(config.EventBustConnectionString);   
            _topicClient =  this.CreateTopicClient();
            _logger = serviceProvider.GetService(typeof(ILogger<EventBusAzureServiceBus>)) as ILogger<EventBusAzureServiceBus>;
        }

        private ITopicClient CreateTopicClient()
        {
            if(this._topicClient == null || this._topicClient.IsClosedOrClosing)
            {
                this._topicClient = new TopicClient(EventBusConfig.EventBustConnectionString, EventBusConfig.DefaultTopicName,RetryPolicy.Default);
            }

            //Ensure that topic already exists
            if(!_managementClient.TopicExistsAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult())
                 _managementClient.CreateTopicAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult();
            return this._topicClient;
        }



        public override void Publish(IntegrationEvent @event)
        {
            var eventName = @event.GetType().Name; // OrderCreatedIntegrationEvent
            eventName = base.ProcessEventName(eventName);

            var eventStr = JsonConvert.DeserializeObject<string>(eventName);
            var eventArr = Encoding.UTF8.GetBytes(eventStr);

            var message = new Message
            {
                MessageId = Guid.NewGuid().ToString(),
                Body = eventArr,
                Label = eventName
            };
            _topicClient.SendAsync(message).GetAwaiter().GetResult();
        }

        public override void Subscribe<T, TH>()
        {

            var eventName = typeof(T).Name;
            eventName = base.ProcessEventName(eventName);

            if (!SubscriptionManager.HasSubscriptionForEvent(eventName))
            {
                var subscriptionClient = CreateSubscriptionClientIfNotExists(eventName);

                RegisterSubscriptionClientMessageHandler(subscriptionClient);
            }

            _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).Name);
            SubscriptionManager.AddSubscription<T, TH>();
            
        }

        private void RegisterSubscriptionClientMessageHandler(ISubscriptionClient subscriptionClient)
        {
            subscriptionClient.RegisterMessageHandler(
                async (message, token) =>
                {
                    var eventName = $"{message.Label}";
                    var messageData = Encoding.UTF8.GetString(message.Body);

                    //Complete the message so that it is not received again.
                    if (await ProcessEvent(ProcessEventName(eventName), messageData))
                    {
                        await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);

                    }
                },
                new MessageHandlerOptions(ExceptionReceivedHandler) { MaxConcurrentCalls = 10, AutoComplete = false });
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var ex = exceptionReceivedEventArgs.Exception;
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;

            _logger.LogError(ex, "ERROR handling message : {ExceptionMessage} - Context : {@ExceptionContext}", ex.Message, context);

            return Task.CompletedTask;
        }

        public override void UnSubscribe<T, TH>()
        {
            var eventName = typeof(T).Name;
            try
            {
                var subscriptionClient = CreateSubscriptionClient(eventName);

                subscriptionClient
                    .RemoveRuleAsync(eventName)
                    .GetAwaiter().GetResult();
            }
            catch (MessagingEntityNotFoundException)
            {

                _logger.LogWarning("The messaging entity {eventName} couldnt be found!", eventName);
            }
            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);
            SubscriptionManager.RemoveSubscription<T,TH>();           
        }

        private ISubscriptionClient CreateSubscriptionClientIfNotExists(string eventName)
        {
            var subClient = CreateSubscriptionClient(eventName);

            bool exists = _managementClient.SubscriptionExistsAsync(EventBusConfig.DefaultTopicName, base.GetSubName(eventName)).GetAwaiter().GetResult();
            if (!exists)
            {
                _managementClient.CreateSubscriptionAsync(EventBusConfig.DefaultTopicName, base.GetSubName(eventName)).GetAwaiter().GetResult();
                RemoveDefaultRule(subClient);
            }
            CreateRuleIfNotExists(base.ProcessEventName(eventName), subClient);
            return subClient;
        }

        private void CreateRuleIfNotExists(string eventName, ISubscriptionClient subscriptionClient)
        {
            bool ruleExist;
            try
            {
                var rule = _managementClient.GetRuleAsync(EventBusConfig.DefaultTopicName, eventName, eventName).GetAwaiter().GetResult();
                ruleExist = rule != null;

            }
            catch (MessagingEntityNotFoundException)
            {

                ruleExist = false;
            }
            if (!ruleExist)
            {
                subscriptionClient.AddRuleAsync(new RuleDescription
                {
                    Filter = new CorrelationFilter { Label = eventName },
                    Name = eventName
                }).GetAwaiter().GetResult();
            }
        }


        private void RemoveDefaultRule(SubscriptionClient subscriptionClient)
        {
            try
            {
                subscriptionClient
                    .RemoveRuleAsync(RuleDescription.DefaultRuleName)
                    .GetAwaiter().GetResult();

            }
            catch (Exception)
            {

                this._logger.LogWarning("The messaging entity {DefaultRuleName} couldnt be found}", RuleDescription.DefaultRuleName);
            }
        }

        private SubscriptionClient CreateSubscriptionClient(string eventName)
        {
            return new SubscriptionClient(EventBusConfig.EventBustConnectionString, EventBusConfig.DefaultTopicName, base.GetSubName(eventName));
        }

        public override void Dispose()
        {
            this._managementClient.CloseAsync().GetAwaiter().GetResult();
            this._topicClient.CloseAsync().GetAwaiter().GetResult();
            this._managementClient = null;
            this._topicClient = null;
            base.Dispose();
        }
    }
}
