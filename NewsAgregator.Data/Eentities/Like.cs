using NewsAgregator.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace NewsAgregator.Data.Eentities
{
    public class Like : IBaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid ArticleId { get; set; }
        public Article Article { get; set; }
    }
}