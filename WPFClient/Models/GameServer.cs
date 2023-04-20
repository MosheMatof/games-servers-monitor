using BL.BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFClient.Contracts.Models;

namespace WPFClient.Models
{
    public class GameServer : IGameServer
    {
        public List<BEGameServer> GameServers { get; set; } = new List<BEGameServer>();
    }
}
