using IGDB.Infrastructure.Massaging.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGDB.Infrastructure.Massaging.Services
{
    public interface IRabbitMQService
    {
        void Publish<T>(T message, string exchangeName, string routingKey) where T : class;
        void Subscribe<T>(string exchangeName, string queueName, string routingKey, Func<T, Task> onMessageReceivedAsync) where T : class;
        void GetNewGamesResponse(NewGamesRequest request, List<int> gamesIds);
        void GetNewGamesRequest(Func<NewGamesRequest, Task> onMessageReceivedAsync);
    }
}
