using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAgent.Contracts
{
    public interface IEmulatorService //: IHostedService, IDisposable
    {
        Task<bool> StartEmulator(int numOfGames, int numOfServers, int interval);
        void StopEmulator();
    }
}
