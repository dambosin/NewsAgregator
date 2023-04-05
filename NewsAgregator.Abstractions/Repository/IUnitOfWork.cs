using NewsAgregator.Data.Eentities;

namespace NewsAgregator.Abstractions.Repository
{
    public interface IUnitOfWork
    {
        public IRepository<Article> Articles { get; }
        public IRepository<Comment> Comments { get; }
        public IRepository<Like> Likes { get; }
        public IRepository<Source> Sources { get; }
        public IRepository<User> Users { get; }
    }
}
