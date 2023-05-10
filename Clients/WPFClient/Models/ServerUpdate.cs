using BL.BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFClient.Contracts.Models;

namespace WPFClient.Models
{
    public class ServerUpdate : IServerUpdate
    {
        private readonly Dictionary<int, List<BEServerUpdate>> servers = new Dictionary<int, List<BEServerUpdate>>();

        public List<BEServerUpdate> this[int serverId]
        {
            get
            {
                if (!servers.TryGetValue(serverId, out List<BEServerUpdate> updateServers))
                {
                    updateServers = new List<BEServerUpdate>();
                    servers.Add(serverId, updateServers);
                }

                return updateServers;
            }
            set
            {
                servers[serverId] = value;
            }
        }
    }
}
