using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NewsAgregator.Data.Eentities;

namespace NewsAgregator.Data
{
    public class NewsAgreratorContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<User> Users { get; set; }
        public NewsAgreratorContext(DbContextOptions<NewsAgreratorContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}