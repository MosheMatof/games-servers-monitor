using BL.BE;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAgent.Contracts
{
    public interface IHubConnectionService : IDisposable, IHostedService
    {
        Task SendMessageAsync<T>(string methodName, T message);
        IAsyncEnumerable<T> GetAllAsync<T>(string methodName, object? arg = null, object? arg2 = null);
        IAsyncEnumerable<T> GetAllLiveAsync<T>(string methodName, Action<T> handler, object? arg = null, object? arg2 = null);
        void On<T>(string methodName, Action<T> handler);
        void StopLiveUpdate();
        void SetTimeout(int? timeout);
    }
}
