using System.ComponentModel.DataAnnotations;

namespace NewsAgregator.Data.Entities
{
    public class Like : IBaseEntity
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; }
        [Required]
        public Guid ArticleId { get; set; }
        public Article Article { get; set; }
    }
}