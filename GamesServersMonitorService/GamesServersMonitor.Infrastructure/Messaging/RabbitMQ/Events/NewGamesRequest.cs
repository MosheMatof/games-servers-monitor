namespace GamesServersMonitor.Infrastructure.Messaging.RabbitMQ.Events
{
    public class NewGamesRequest
    {
        public int Amount { get; set; }
        public string CorrelationId { get; set; }
        public string QueueName { get; set; } = "AmountGamesIds";
        //public string? SortField { get; set; }
        //public string? Fields { get; set; }
    }
}