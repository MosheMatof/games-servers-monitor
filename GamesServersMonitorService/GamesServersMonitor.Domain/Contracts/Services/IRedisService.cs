using GamesServersMonitor.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesServersMonitor.Domain.Contracts.Services
{
    public interface IRedisService
    {
        Task<bool> SetServerUpdateAsync(ServerUpdate serverUpdate);
        Task<ServerUpdate> GetServerUpdateAsync(int id);
        //Task<IAsyncEnumerable<ServerUpdate>> GetAllServerUpdatesAsync();
    }
}
