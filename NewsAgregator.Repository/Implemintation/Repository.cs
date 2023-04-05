using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NewsAgregator.Abstractions;
using NewsAgregator.Core.Dto;
using System.Linq.Expressions;

namespace NewsAgregator.Repository.Implemintation
{
    public class Repository<T> : IRepository<T> where T : class, IBaseEntity
    {
        private readonly DbContext _db;
        private readonly DbSet<T> _dbset;

        public Repository(DbContext db)
        {
            _db = db;
            _dbset = _db.Set<T>();
        }
        public async Task<T?> GetByIdAsync(Guid id) => await _dbset.AsNoTracking().SingleOrDefaultAsync(entity => entity.Id == id);
        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var result = _dbset.Where(predicate).AsQueryable();
            if (includes.Any())
            {
                result = includes
                    .Aggregate(result,
                        (current, include)
                            => current.Include(include));
            }
            return result;
        }
        public IQueryable<T> GetAsQueryable() => _dbset.AsQueryable();

        public async Task<EntityEntry<T>> AddAsync(T entity) => await _dbset.AddAsync(entity);
        public async Task AddRangeAsync(IEnumerable<T> entities) => await _dbset.AddRangeAsync(entities);

        public void Update(T entity) => _dbset.Update(entity);
        public async void PatchAsync(Guid id, List<PatchDto> patches)
        {
            var entity = await _dbset
                .SingleOrDefaultAsync(entity => entity.Id == id);
            var nameValuePairs = patches
                .ToDictionary(k => k.PropertyName, v => v.Value);
            var dbEntry = _db.Entry(entity);
            dbEntry.CurrentValues.SetValues(nameValuePairs);
            dbEntry.State = EntityState.Modified;
        }

        public void RemoveRange(IEnumerable<T> entities) => _dbset.RemoveRange(entities);
        public async Task Remove(Guid id) => _dbset
            .Remove(await _dbset
                .FirstOrDefaultAsync(entity => entity.Id == id));

        public void Dispose()
        {
            _db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
