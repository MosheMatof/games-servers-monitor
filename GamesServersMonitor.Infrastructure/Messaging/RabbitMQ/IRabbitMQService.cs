using GamesServersMonitor.Infrastructure.Messaging.RabbitMQ.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesServersMonitor.Infrastructure.Messaging.RabbitMQ
{
    public interface IRabbitMQService
    {
        void Publish<T>(T message, string exchangeName, string routingKey) where T : class;
        Task Subscribe<T>(string exchangeName, string queueName, string routingKey) where T : class;
        //Task Subscribe<T>(string exchangeName, string queueName, string routingKey, Func<T, Task> onMessageReceivedAsync) where T : class;
        Task GetNewGamesRequest(NewGamesRequest request, Func<NewGamesResponse, Task> onMessageReceivedAsync);
    }
}
