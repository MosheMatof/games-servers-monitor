using Microsoft.AspNetCore.Mvc;

namespace GamesServersMonitor.App.Services
{
    public interface IEmulatorService
    {
        Task StartAsync(int numOfServers, List<int> gameIds, int _interval, Func<bool, IActionResult> callback);
        Task<string> StopAsync();
    }
}