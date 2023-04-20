using GamesServersMonitor.Domain.Entities;

namespace GamesServersMonitor.Domain.Contracts.Services
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<GameServer> GameServerRepository { get; }
        IRepository<ServerUpdate> ServerUpdateRepository { get; }
        Task SaveAsync();
    }
}
