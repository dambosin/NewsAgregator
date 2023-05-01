using System.ComponentModel.DataAnnotations;

namespace NewsAgregator.Data.Entities
{
    public class Comment : IBaseEntity
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
        public string? Text { get; set; }
        public DateTime? Created { get; set; }
        public Guid? ParentCommentId { get; set; }
        public Comment? ParentComment { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; }
        [Required]
        public Guid ArticleId { get; set; }
        public Article Article { get; set; }
    }
}