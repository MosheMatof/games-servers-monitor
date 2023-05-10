namespace GamesServersMonitor.App.Services
{
    public interface IGetGamesService
    {
        Task<List<int>> GetNewGamesAsync(int amount, int timeout = 10000);
    }
}