namespace GamesServersMonitor.Domain.Entities
{
    public class ServerUpdate
    {
        public int ServerId { get; set; }
        public int CurrentPlayers { get; set; }
        public bool IsRunning { get; set; }
        public float CpuTemperature { get; set; }
        public float CpuSpeed { get; set; }
        public float HighScore { get; set; }
        public float AvgScore { get; set; }
        public float MemoryUsage { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
