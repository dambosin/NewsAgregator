﻿using NewsAgregator.Data.Entities;

namespace NewsAgregator.Abstractions.Repository
{
    public interface IUnitOfWork
    {
        public IRepository<Article> Articles { get; }
        public IRepository<Comment> Comments { get; }
        public IRepository<Like> Likes { get; }
        public IRepository<Source> Sources { get; }
        public IRepository<User> Users { get; }
        public IRepository<Role> Roles { get; }
        public IRepository<UserRole> UserRoles { get; }

        public Task<int> CommitAsync();
    }
}
