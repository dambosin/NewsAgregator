using System.ComponentModel.DataAnnotations;

namespace NewsAgregator.Mvc.Models.Comments
{
    public class CommentCreateModel
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string Text { get; set; }
        public Guid? ParrentCommentId { get; set; }
    }
}
