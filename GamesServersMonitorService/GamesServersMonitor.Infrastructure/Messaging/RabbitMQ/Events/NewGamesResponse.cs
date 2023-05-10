using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesServersMonitor.Infrastructure.Messaging.RabbitMQ.Events
{
    public class NewGamesResponse
    {
        public List<int> GamesIds { get; set; }
        public string CorrelationId { get; set; }
        public string QueueName { get; set; } = "ListGamesIds";
    }
}
