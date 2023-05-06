using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis.Extensions.Core;
using System.Text.Json;
using System.Threading.Tasks;
using GamesServersMonitor.Domain.Contracts.Services;
using GamesServersMonitor.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace GamesServersMonitor.Infrastructure.Redis
{
    public class RedisService : IRedisService
    {
        private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IDatabase _db;

        public RedisService(IDistributedCache cache, IConnectionMultiplexer multiplexer)
        {
            _cache = cache;
            _connectionMultiplexer = multiplexer;
            _db = _connectionMultiplexer.GetDatabase();
        }

        public async Task<bool> SetServerUpdateAsync(ServerUpdate serverUpdate)
        {
            if (serverUpdate == null)
            {
                return false;
            }

            string key = $"ServerUpdate:{serverUpdate.ServerId}";
            string serializedServerUpdate = JsonSerializer.Serialize(serverUpdate);

            //await _cache.SetStringAsync(key, serializedServerUpdate);
            await _db.StringSetAsync(key, serializedServerUpdate);
            return true;
        }

        public async Task<ServerUpdate> GetServerUpdateAsync(int id)
        {
            string key = $"ServerUpdate:{id}";
            string serializedServerUpdate = await _cache.GetStringAsync(key);

            if (string.IsNullOrEmpty(serializedServerUpdate))
            {
                return null;
            }

            ServerUpdate serverUpdate = JsonSerializer.Deserialize<ServerUpdate>(serializedServerUpdate);

            return serverUpdate;
        }

        //public async IAsyncEnumerable<ServerUpdate> GetAllServerUpdatesAsync()
        //{
        //    var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.Configuration);

        //    // get all the keys matching "ServerUpdate:*"
        //    var keys = await _db.GetKeysAsync("ServerUpdate:*");

        //    if (keys == null || !keys.Any())
        //    {
        //        throw new ArgumentNullException();
        //    }
        //    foreach (var key in keys)
        //    {
        //        string serializedServerUpdate = await _cache.GetStringAsync(key);
        //        if (!string.IsNullOrEmpty(serializedServerUpdate))
        //        {
        //            ServerUpdate serverUpdate = JsonSerializer.Deserialize<ServerUpdate>(serializedServerUpdate);
        //            yield return serverUpdate;
        //        }
        //    }
        //}
    }
}
