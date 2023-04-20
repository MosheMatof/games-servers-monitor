using BL.BE;

namespace ServiceAgent.Contracts
{
    public interface IGameService
    {
        IAsyncEnumerable<BEGame> GetGameInfoAsync(List<int> gameIds);
        Task<BEGame> GetGameInfoAsync(int gameId);
    }
}