using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client.Exceptions;
using GamesServersMonitor.Infrastructure.Messaging.RabbitMQ.Events;

namespace GamesServersMonitor.Infrastructure.Messaging.RabbitMQ
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
            _logger.LogInformation("'{_factory.ClientProvidedName}' service: Published message to exchange '{exchangeName}' with routing key '{routingKey}'", _factory.ClientProvidedName, exchangeName, routingKey);
        }

        public async Task Subscribe<T>(string exchangeName, string queueName, string routingKey) where T : class
        {
            InitConnection();
            ExchangeDeclare(exchangeName);
            QueueDeclare(queueName);
            _channel.QueueBind(queueName, exchangeName, routingKey);
            //_consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += handleMessageReceived;
            //_consumer.Received += async (sender, args) =>
            //{
            //    var jsonMessage = Encoding.UTF8.GetString(args.Body.ToArray());
            //    var message = JsonSerializer.Deserialize<T>(jsonMessage);

            //    _logger.LogInformation("'{_factory.ClientProvidedName}' service: Received message from exchange '{exchangeName}' with routing key '{routingKey}' and delivery tag {deliveryTag}", _factory.ClientProvidedName, exchangeName, routingKey, args.DeliveryTag);

            //    await onMessageReceivedAsync(message);
            //    _channel.BasicAck(args.DeliveryTag, false);
            //};
            await Task.Run(() => _channel.BasicConsume(queueName, false, _consumer));
            _logger.LogInformation("'{_factory.ClientProvidedName}' service: Subscribed to queue '{queueName}' on exchange '{exchangeName}' with routing key '{routingKey}'", _factory.ClientProvidedName, queueName, exchangeName, routingKey);
        }



        public async Task GetNewGamesRequest(NewGamesRequest request, Func<NewGamesResponse, Task> messageHandlerAsync)
        {
            var sourceExchangeName = "game_exchange_src";
            var destinationExchangeName = "game_exchange_dest";
            var routingKey = "game.newgames.request";

            if (_consumer == null)
            {
                _consumer = new EventingBasicConsumer(_channel);
                var responseQueueName = request.QueueName;
                var responseRoutingKey = "game.newgames.response";
                await Subscribe<NewGamesResponse>(destinationExchangeName, responseQueueName, responseRoutingKey);
            }
            // Send the request message
            OnMessageReceivedAsync = messageHandlerAsync;
            Publish(request, sourceExchangeName, routingKey);
        }

        //public async Task GetNewGamesRequest(NewGamesRequest request, Func<NewGamesResponse, Task> messageHandlerAsync)
        //{
        //    var sourceExchangeName = "game_exchange_src";
        //    var destinationExchangeName = "game_exchange_dest";
        //    var routingKey = "game.newgames.request";
        //    var correlationId = request.CorrelationId;
        //    _logger.LogInformation(message: $"correlationId: {correlationId}");
        //    // Send the request message
        //    Publish(request, sourceExchangeName, routingKey);

        //    // Listen for response messages
        //    var responseQueueName = request.QueueName;
        //    var responseRoutingKey = $"game.newgames.response.{correlationId}";

        //    await Subscribe<NewGamesResponse>(destinationExchangeName, responseQueueName, responseRoutingKey, async response =>
        //    {
        //        _logger.LogInformation(message: $"correlationId: {correlationId}");
        //        // Check if this response message is for the current request
        //        if (response.CorrelationId == correlationId)
        //        {
        //            // Invoke the callback with the deserialized response message
        //            await messageHandlerAsync(response);
        //        }
        //        else
        //        {
        //            //log warnning the unmatching correlationId
        //            _logger.LogWarning($"the correlation Id of the recived message doesn't match");
        //        }
        //    });
        //}
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

        private async void handleMessageReceived(object? sender, BasicDeliverEventArgs e)
        {
            var jsonMessage = Encoding.UTF8.GetString(e.Body.ToArray());
            var message = JsonSerializer.Deserialize<NewGamesResponse>(jsonMessage);

            _logger.LogInformation("'{_factory.ClientProvidedName}' service: Received message from exchange '{exchangeName}' with routing key '{routingKey}' and delivery tag {deliveryTag}", _factory.ClientProvidedName, e.Exchange, e.RoutingKey, e.DeliveryTag);

            await OnMessageReceivedAsync(message);
            _channel.BasicAck(e.DeliveryTag, false);
        }

        private Func<NewGamesResponse, Task> _onMessageReceivedAsync;

        private Func<NewGamesResponse, Task> OnMessageReceivedAsync
        {
            get { return _onMessageReceivedAsync; }
            set { _onMessageReceivedAsync = value; }
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