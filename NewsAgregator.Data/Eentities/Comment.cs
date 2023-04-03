using System.ComponentModel.DataAnnotations;

namespace NewsAgregator.Data.Eentities
{
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }
        public string? Text { get; set; }
        public DateTime Created { get; set; }
        public Guid? ParentCommentId { get; set; }
        public Comment? ParentComment { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid ArticleId { get; set; }
        public Article Article { get; set; }
    }
}