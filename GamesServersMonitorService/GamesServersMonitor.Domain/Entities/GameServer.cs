namespace GamesServersMonitor.Domain.Entities
{
    public class GameServer
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public string Name { get; set; }
        public int CurrentPlayers { get; set; }
        public int PlayersCapacity { get; set; }
        public string GameMode { get; set; }
        public float HighScore { get; set; }
        public float AvgScore { get; set; }
        public bool IsRunning { get; set; }
        public float CpuTemperature { get; set; }
        public float CpuSpeed { get; set; }
        public float MemoryUsage { get; set; }
        public float MemoryCapacity { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
