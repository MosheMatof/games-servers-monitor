using GamesServersMonitor.Infrastructure.Messaging.MediatR;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GamesServersMonitor.App.Services
{
    public interface IEmulatorService : IRequestHandler<ServerUpdateRequest>
    {
        Task ResumeAsync(Action<bool> callback);
        Task StartAsync(int numOfServers, List<int> gameIds, int _interval, Action<bool> callback);
        Task<string> StopAsync();
    }
}