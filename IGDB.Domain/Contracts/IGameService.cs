using System.Threading.Tasks;

namespace IGDB.Domain.Contracts
{
    public interface IGameService
    {
        Task<List<int>> GetGamesIdsAsync(int number);
        Task<string> GetGameInfoByIdAsync(int gameId);
        Task<List<T>> GetGamesAsync<T>(int amount, string sortField = "total_rating_count", string fields = "id");
    }
}