using Microsoft.EntityFrameworkCore;
using NewsAgregator.Abstractions.Repository;
using NewsAgregator.Data;
using NewsAgregator.Data.Entities;

namespace NewsAgregator.Repository
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly NewsAgregatorContext _db;
        private readonly IRepository<Article> _aritcles;
        private readonly IRepository<Comment> _comments;
        private readonly IRepository<Like> _likes;
        private readonly IRepository<Source> _sources;
        private readonly IRepository<User> _users;
        private readonly IRepository<Role> _roles;
        private readonly IRepository<UserRole> _userRoles;

        public UnitOfWork(NewsAgregatorContext db,
            IRepository<Article> aritcles,
            IRepository<Comment> comments,
            IRepository<Like> likes,
            IRepository<Source> sources,
            IRepository<User> users,
            IRepository<Role> roles,
            IRepository<UserRole> userRoles)
        {
            _db = db;
            _aritcles = aritcles;
            _comments = comments;
            _likes = likes;
            _sources = sources;
            _users = users;
            _roles = roles;
            _userRoles = userRoles;
        }

        public IRepository<Article> Articles => _aritcles;

        public IRepository<Comment> Comments => _comments;

        public IRepository<Like> Likes => _likes;

        public IRepository<Source> Sources => _sources;

        public IRepository<User> Users => _users;
        public IRepository<Role> Roles => _roles;
        public IRepository<UserRole > UserRoles => _userRoles;

        public async Task<int> CommitAsync()
        {
            return await _db.SaveChangesAsync();
        }

        public void Dispose()
        {
            _db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
