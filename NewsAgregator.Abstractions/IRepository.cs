using Microsoft.EntityFrameworkCore.ChangeTracking;
using NewsAgregator.Core.Dto;
using System.Linq.Expressions;

namespace NewsAgregator.Abstractions
{
    public interface IRepository<T> : IDisposable where T : class, IBaseEntity
    {
        //read
        Task<T?> GetByIdAsync(Guid id);
        IQueryable<T> FindBy(
            Expression<Func<T, bool>> predicate, 
            params Expression<Func<T, Object>>[] includes);
        IQueryable<T> GetAsQueryable();

        //create
        Task<EntityEntry<T>> AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        //update
        void Update(T entity);
        void PatchAsync(Guid id, List<PatchDto> patches);

        //remove
        Task Remove(Guid id);
        void RemoveRange(IEnumerable<T> entities);

    }
}