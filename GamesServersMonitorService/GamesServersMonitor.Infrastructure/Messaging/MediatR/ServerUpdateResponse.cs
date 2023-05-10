using GamesServersMonitor.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesServersMonitor.Infrastructure.Messaging.MediatR
{
    public class ServerUpdateResponse : IRequest
    {
        public ServerUpdate ServerUpdate { get; set; }
    }
}
