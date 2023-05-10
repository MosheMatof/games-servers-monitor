using System;

namespace BL.BE
{
    public class BEGameServer
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

        public BEGameServer() { }
        //copy constructor
        public BEGameServer(BEGameServer other)
        {
            Id = other.Id;
            GameId = other.GameId;
            Name = other.Name;
            CurrentPlayers = other.CurrentPlayers;
            PlayersCapacity = other.PlayersCapacity;
            GameMode = other.GameMode;
            HighScore = other.HighScore;
            AvgScore = other.AvgScore;
            IsRunning = other.IsRunning;
            CpuTemperature = other.CpuTemperature;
            CpuSpeed = other.CpuSpeed;
            MemoryUsage = other.MemoryUsage;
            MemoryCapacity = other.MemoryCapacity;
            TimeStamp = other.TimeStamp;
        }

        public BEGameServer Update(BEServerUpdate update)
        {
            Id = update.ServerId;
            CurrentPlayers = update.CurrentPlayers;
            HighScore = update.HighScore;
            AvgScore = update.AvgScore;
            IsRunning = update.IsRunning;
            CpuTemperature = update.CpuTemperature;
            CpuSpeed = update.CpuSpeed;
            MemoryUsage = update.MemoryUsage;
            TimeStamp = update.TimeStamp;

            return this;
        }
    }
}