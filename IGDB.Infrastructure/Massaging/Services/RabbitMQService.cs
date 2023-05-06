using IGDB.Infrastructure.Massaging.Events;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IGDB.Infrastructure.Massaging.Services
{
    public class RabbitMQService : IRabbitMQService, IDisposable
    {
        private readonly ILogger<RabbitMQService> _logger;
        private readonly ConnectionFactory _factory;
        private IConnection? _connection;
        private IModel? _channel;
        private EventingBasicConsumer _consumer;


        public RabbitMQService(ILogger<RabbitMQService> logger, ConnectionFactory factory)
        {
            _logger = logger;
            _factory = factory;
        }

        public void Publish<T>(T message, string exchangeName, string routingKey) where T : class
        {
            InitConnection();
            ExchangeDeclare(exchangeName);
            var jsonMessage = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);
            _channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: null, body: body);

            _logger.LogInformation("'{_factory.ClientProvidedName}' service: Published message {Message} to exchange {Exchange} with routing key {RoutingKey}", _factory.ClientProvidedName, message, exchangeName, routingKey);
        }

        public void Subscribe<T>(string exchangeName, string queueName, string routingKey, Func<T, Task> onMessageReceivedAsync) where T : class
        {
            InitConnection();
            ExchangeDeclare(exchangeName);
            QueueDeclare(queueName);
            _channel.QueueBind(queueName, exchangeName, routingKey);
            _consumer.Received += async (sender, args) =>
            {
                var jsonMessage = Encoding.UTF8.GetString(args.Body.ToArray());
                var message = JsonSerializer.Deserialize<T>(jsonMessage);

                _logger.LogInformation("'{_factory.ClientProvidedName}' service: Received message {Message} from exchange {Exchange} with routing key {RoutingKey}", _factory.ClientProvidedName, message, exchangeName, routingKey);

                await onMessageReceivedAsync(message);
                _channel.BasicAck(args.DeliveryTag, false);
            };
            _channel.BasicConsume(queueName, false, _consumer);

            _logger.LogInformation("'{_factory.ClientProvidedName}' service: Subscribed to queue {Queue} with routing key {RoutingKey} on exchange {Exchange}", _factory.ClientProvidedName, queueName, routingKey, exchangeName);
        }

        public void GetNewGamesResponse(NewGamesRequest request, List<int> gamesIds)
        {
            var response = new NewGamesResponse { GamesIds = gamesIds, CorrelationId = request.CorrelationId };
            var exchangeName = "game_exchange_dest";
            var routingKey = $"game.newgames.response";
            Publish(response, exchangeName, routingKey);
        }

        public void GetNewGamesRequest(Func<NewGamesRequest, Task> onMessageReceivedAsync)
        {
            if (_consumer == null)
            {
                _consumer = new EventingBasicConsumer(_channel);

                var exchangeName = "game_exchange_src";
                var queueName = "GetGamesIds";
                var routingKey = "game.newgames.request";
                Subscribe<NewGamesRequest>(exchangeName, queueName, routingKey, onMessageReceivedAsync);
            }
        }

        private void InitConnection()
        {
            if (_connection != null && _connection.IsOpen && _channel != null && _channel.IsOpen)
                return;
            _logger.LogInformation("'{_factory.ClientProvidedName}' service: Starting _connection to RabbitMQ broker at {_factory.HostName}:{_factory.Port} ...", _factory.ClientProvidedName, _factory.HostName, _factory.Port);
            try
            {
                _connection = _factory.CreateConnection();
                if (_connection.IsOpen)
                {
                    _logger.LogInformation("'{_factory.ClientProvidedName}' service: Connection to RabbitMQ broker established successfully", _factory.ClientProvidedName);
                }
                else
                {
                    _logger.LogError("'{_factory.ClientProvidedName}' service: Failed to establish _connection to RabbitMQ broker", _factory.ClientProvidedName);
                }
                _channel = _connection.CreateModel();
            }
            catch (Exception ex)
            {
                _logger.LogError("'{_factory.ClientProvidedName}' service: Error establishing _connection to RabbitMQ broker - {ErrorMessage}", _factory.ClientProvidedName, ex.Message);
            }
        }

        private void ExchangeDeclare(string exchangeName)
        {
            try
            {
                _channel.ExchangeDeclarePassive(exchangeName);
            }
            catch (OperationInterruptedException ex)
            {
                if (ex.ShutdownReason.ReplyCode == 404)
                {
                    // The exchange does not exist, so we need to declare it.
                    _channel.Dispose();
                    InitConnection();
                    _channel.ExchangeDeclare(exchangeName, ExchangeType.Topic, durable: true, autoDelete: false);
                }
                else
                {
                    throw ex;
                }
            }
        }

        private void QueueDeclare(string queueName)
        {
            try
            {
                _channel.QueueDeclarePassive(queueName);
            }
            catch (OperationInterruptedException ex)
            {
                if (ex.ShutdownReason.ReplyCode == 404)
                {
                    // The queue does not exist, so we need to declare it.
                    _channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                }
                else
                {
                    throw ex;
                }
            }
        }

        public void Dispose()
        {
            DisposeChannel();
            DisposeConnection();

            _logger.LogInformation("Disposed RabbitMQ service '{_factory.ClientProvidedName}'", _factory.ClientProvidedName);
        }

        private void DisposeChannel()
        {
            try
            {
                _channel?.Close();
                _channel?.Dispose();
                _logger.LogInformation("Disposed RabbitMQ _channel for service '{_factory.ClientProvidedName}'", _factory.ClientProvidedName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to dispose RabbitMQ _channel for service '{_factory.ClientProvidedName}'", _factory.ClientProvidedName);
            }
        }

        private void DisposeConnection()
        {
            try
            {
                _connection?.Close();
                _connection?.Dispose();
                _logger.LogInformation("Disposed RabbitMQ _connection for service '{_factory.ClientProvidedName}'", _factory.ClientProvidedName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to dispose RabbitMQ _connection for service '{_factory.ClientProvidedName}'", _factory.ClientProvidedName);
            }
        }
    }
}
