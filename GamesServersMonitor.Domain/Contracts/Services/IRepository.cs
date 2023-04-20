using System.Linq.Expressions;

namespace GamesServersMonitor.Domain.Contracts.Services
{
    public interface IRepository<T> : IDisposable
    {
        Task<IAsyncEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string includeProperties = "");
        Task<T> GetByIdAsync(object id);
        Task AddAsync(T t);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T t);
        Task DeleteAllAsync();
        Task DeleteAsync(object id);
    }
}
