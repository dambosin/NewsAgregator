using Microsoft.EntityFrameworkCore.ChangeTracking;
using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Entities;
using System.Linq.Expressions;

namespace NewsAgregator.Abstractions.Repository
{
    public interface IRepository<T> : IDisposable where T : class, IBaseEntity
    {
        //read
        /// <summary>
        /// Get entity from database by id
        /// </summary>
        /// <param name="id">Id of entity</param>
        /// <returns>Entity if exist or null</returns>
        Task<T?> GetByIdAsync(Guid id);
        /// <summary>
        /// Get entities filtered and includes other entities
        /// </summary>
        /// <param name="predicate">Filter expression</param>
        /// <param name="includes">Entities to include</param>
        /// <returns>IQueryable collection of entities</returns>
        IQueryable<T> FindBy(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes);
        /// <summary>
        /// Get all entities as Quearyable collection
        /// </summary>
        /// <returns>Queryable collectiob of entities</returns>
        IQueryable<T> GetAsQueryable();

        //create
        /// <summary>
        /// Add entity to database
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <returns>Entry of added entity</returns>
        Task<EntityEntry<T>> AddAsync(T entity);
        /// <summary>
        /// Add collection of entities to database
        /// </summary>
        /// <param name="entities">Entities to add</param>
        /// <returns>Completed task</returns>
        Task AddRangeAsync(IEnumerable<T> entities);

        //update
        /// <summary>
        /// Update entity in database
        /// </summary>
        /// <param name="entity">Entity to update</param>
        void Update(T entity);
        /// <summary>
        /// Patch fields of entity
        /// </summary>
        /// <param name="id">Id of entity to patch</param>
        /// <param name="patches">Fields to change</param>
        void Patch(Guid id, List<PatchDto> patches);

        //remove
        /// <summary>
        /// Delete entity from database
        /// </summary>
        /// <param name="id">Id of entity to delete</param>
        /// <returns>Completed task</returns>
        Task RemoveAsync(Guid id);
        /// <summary>
        /// Delete collection of entities from database
        /// </summary>
        /// <param name="entities">Collection of entities</param>
        void RemoveRange(IEnumerable<T> entities);

        //utils
        /// <summary>
        /// Count entities in database
        /// </summary>
        /// <returns>Total number of entities</returns>
        Task<int> CountAsync();
    }
}