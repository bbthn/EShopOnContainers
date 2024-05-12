
using EventBus.Base;
using EventBus.Base.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;

namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMQ : BaseEventBus
    {
        public RabbitMQPersistentConnection persistentConnection;
        private readonly IConnectionFactory connectionFactory;
        private readonly IModel _consumerChannel;
        public EventBusRabbitMQ(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
        {
            if (config.Connection != null)
                connectionFactory = (ConnectionFactory)config.Connection;
            else
                connectionFactory = new ConnectionFactory();

            ILogger<RabbitMQPersistentConnection> logger = serviceProvider.GetService(typeof(ILogger<RabbitMQPersistentConnection>)) as ILogger<RabbitMQPersistentConnection>;
            persistentConnection = new RabbitMQPersistentConnection(connectionFactory, config.ConnectionRetryCount, logger);

            _consumerChannel = CreateConsumerChannel();

            SubscriptionManager.onEventRemoved += SubsManager_OnEventRemoved;

        }

        private void SubsManager_OnEventRemoved(object? sender, string eventName)
        {
            eventName = ProcessEventName(eventName);
            if (!persistentConnection.IsConnected)
                persistentConnection.TryConnect();

            _consumerChannel.QueueUnbind(queue: eventName,
                                        exchange: EventBusConfig.DefaultTopicName,
                                        routingKey: eventName);
            if (SubscriptionManager.IsEmpty)
                _consumerChannel.Close();
        }

        public override void Publish(IntegrationEvent @event)
        {
            if (!persistentConnection.IsConnected)
                persistentConnection.TryConnect();

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(EventBusConfig.ConnectionRetryCount, retryAttemp => TimeSpan.FromSeconds(Math.Pow(2, retryAttemp)),
                (ex, time) =>
                {
                    //todo logging 
                });
            var eventName = @event.GetType().Name;
            eventName = ProcessEventName(eventName);

            _consumerChannel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName, type: "direct"); //ensureExchange

            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            policy.Execute(() =>
            {
                var properties = _consumerChannel.CreateBasicProperties();
                properties.DeliveryMode = 2;//persistent

                _consumerChannel.QueueDeclare(queue: GetSubName(eventName), //ensure queue exists
                                             durable: true,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                _consumerChannel.BasicPublish(
                    exchange: EventBusConfig.DefaultTopicName,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            });
        }

        public override void Subscribe<T, TH>()
        {
            var eventName = typeof(T).Name;
            eventName = ProcessEventName(eventName);


            if (!SubscriptionManager.HasSubscriptionForEvent(eventName))
            {
                if (!persistentConnection.IsConnected)
                    persistentConnection.TryConnect();

                _consumerChannel.QueueDeclare(queue: GetSubName(eventName),
                                              durable: true,
                                              exclusive: false,
                                              autoDelete: false,
                                              arguments: null);

                _consumerChannel.QueueBind(queue: GetSubName(eventName),
                                            exchange: EventBusConfig.DefaultTopicName,
                                            routingKey: eventName);

            }
            base.SubscriptionManager.AddSubscription<T, TH>();
            StartBasicConsume(eventName);

        }

        public override void UnSubscribe<T, TH>()
        {
            base.SubscriptionManager.RemoveSubscription<T, TH>();
        }

        private IModel CreateConsumerChannel()
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }
            var channel = persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName, type: "direct");

            return channel;
        }

        private void StartBasicConsume(string eventName)
        {
            if (_consumerChannel.IsOpen)
            {
                var consumer = new EventingBasicConsumer(_consumerChannel);

                consumer.Received += Consumer_Received;

                _consumerChannel.BasicConsume(queue: GetSubName(eventName),
                                              autoAck: false,
                                              consumer: consumer);
            }
        }

        private async void Consumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            var eventName = e.RoutingKey;
            eventName = ProcessEventName(eventName);
            var message = Encoding.UTF8.GetString(e.Body.Span);
            try
            {
                await ProcessEvent(eventName, message);
            }
            catch (Exception)
            {
            }
            _consumerChannel.BasicAck(e.DeliveryTag, multiple: false);

        }
    }
}
