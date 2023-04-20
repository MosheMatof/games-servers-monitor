using BL.BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient.Contracts.Models
{
    public interface IServerUpdate
    {
        List<BEServerUpdate> this[int serverId] { get; set; }
    }
}
