using GamesServersMonitor.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesServersMonitor.Infrastructure.Messaging.MediatR
{
    public class ServerUpdateRequest : IRequest
    {
        public int? ServerId { get; set; }
        public bool Stop { get; set; }
    }
}
