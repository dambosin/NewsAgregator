using Microsoft.EntityFrameworkCore;
using NewsAgregator.Data.Entities;

namespace NewsAgregator.Data
{
    public class NewsAgregatorContext : DbContext
    {
        public virtual DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public NewsAgregatorContext(DbContextOptions<NewsAgregatorContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>()
                .Property(article => article.PositiveIndex)
                .HasDefaultValue(-10);
            modelBuilder.Entity<Article>()
                .Property(article => article.Created)
                .HasDefaultValueSql("getdate()");
            modelBuilder.Entity<Article>()
                .Property(article => article.LikesCount)
                .HasDefaultValue(0);

            modelBuilder.Entity<Comment>()
                .Property(comment => comment.Created)
                .HasDefaultValueSql("getdate()");
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}