using Microsoft.EntityFrameworkCore.ChangeTracking;
using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Entities;
using System.Linq.Expressions;

namespace NewsAgregator.Abstractions.Repository
{
    public interface IRepository<T> : IDisposable where T : class, IBaseEntity
    {
        //read
        Task<T?> GetByIdAsync(Guid id);
        IQueryable<T> FindBy(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes);
        IQueryable<T> GetAsQueryable();

        //create
        Task<EntityEntry<T>> AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        //update
        void Update(T entity);
        void PatchAsync(Guid id, List<PatchDto> patches);

        //remove
        Task RemoveAsync(Guid id);
        void RemoveRange(IEnumerable<T> entities);

        //utils
        Task<int> CountAsync();
    }
}