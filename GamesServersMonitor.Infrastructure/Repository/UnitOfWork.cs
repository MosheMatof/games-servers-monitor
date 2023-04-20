using GamesServersMonitor.Domain.Entities;
using GamesServersMonitor.Domain.Contracts.Services;
using GamesServersMonitor.Infrastructure;

namespace GamesServersMonitor.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GamesMonitorDbContext context;
        private IRepository<GameServer>? gameServerRepository;
        private IRepository<ServerUpdate>? serverUpdateRepository;

        public UnitOfWork(GamesMonitorDbContext _context)
        {
            this.context = _context;
        }

        public IRepository<GameServer> GameServerRepository
        {
            get
            {
                gameServerRepository ??= new Repository<GameServer>(context);
                return gameServerRepository;
            }
        }

        public IRepository<ServerUpdate> ServerUpdateRepository
        {
            get
            {
                serverUpdateRepository ??= new Repository<ServerUpdate>(context);
                return serverUpdateRepository;
            }
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
