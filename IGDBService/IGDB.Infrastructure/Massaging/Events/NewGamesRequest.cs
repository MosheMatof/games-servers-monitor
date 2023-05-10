namespace IGDB.Infrastructure.Massaging.Events
{
    public class NewGamesRequest
    {
        public int Amount { get; set; }
        public string CorrelationId { get; set; }
        public string QueueName { get; set; } = "AmountGamesIds";
    }
}