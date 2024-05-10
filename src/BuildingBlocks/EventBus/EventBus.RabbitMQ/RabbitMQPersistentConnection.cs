

using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace EventBus.RabbitMQ
{
    public class RabbitMQPersistentConnection : IDisposable
    {
        private IConnection connection;
        private readonly IConnectionFactory connectionFactory;
        private object lock_object = new object();
        private int _retryCount;
        private ILogger<RabbitMQPersistentConnection> _logger;
        private bool _disposed;
        

        public RabbitMQPersistentConnection(IConnectionFactory connectionFactory, int retryCount,ILogger<RabbitMQPersistentConnection> logger)
        {
            this.connectionFactory = connectionFactory;
            this._retryCount = retryCount;
            this._logger = logger;
        }

        public bool IsConnected => connection != null && connection.IsOpen;
        public void Dispose()
        {
            this._disposed=true;
            connection.Dispose();
        }
        public IModel CreateModel()
        {
            return connection.CreateModel();
        }

        public bool TryConnect()
        {
            lock (lock_object)
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                        _logger.LogWarning("RabbitMQ connection attempt was failed!");

                    });

                policy.Execute(() =>
                {
                    connection = connectionFactory.CreateConnection();
                });

                if (IsConnected)
                {
                    connection.ConnectionShutdown += Connection_ConnectionShutdown;
                    connection.ConnectionBlocked += Connection_ConnectionBlocked;
                    connection.CallbackException += Connection_CallbackException;
                    return true;
                }
                return false;
            }
        }

        private void Connection_CallbackException(object? sender, global::RabbitMQ.Client.Events.CallbackExceptionEventArgs e)
        {
            if (this._disposed) return;
            this.TryConnect();
        }

        private void Connection_ConnectionBlocked(object? sender, global::RabbitMQ.Client.Events.ConnectionBlockedEventArgs e)
        {
            if (this._disposed) return;
            this.TryConnect();
        }

        private void Connection_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            if (this._disposed) return;
            this.TryConnect();
        }
    }
}
