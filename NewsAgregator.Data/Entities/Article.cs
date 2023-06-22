using System.ComponentModel.DataAnnotations;

namespace NewsAgregator.Data.Entities
{
    public class Article : IBaseEntity
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string IdOnSite { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string UrlHeader { get; set; }
        [Required]
        public string ShortDescription { get; set; }
        [Required]
        public Guid SourceId { get; set; }
        public Source Source { get; set; }
        [Required]
        public string Content { get; set; }
        public double PositiveIndex { get; set; }
        public List<Comment>? Comments { get; set; }
        [Required]
        public DateTime Created { get; set; }
        [Required]
        public int LikesCount { get; set; }
    }
}
