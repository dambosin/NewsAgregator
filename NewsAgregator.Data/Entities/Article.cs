using System.ComponentModel.DataAnnotations;

namespace NewsAgregator.Data.Entities
{
    public class Article : IBaseEntity
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string UrlHeader { get; set; }
        public string IdOnSite { get; set; }
        public string UrlThumbnail { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? Content { get; set; }
        public double? PositiveIndex { get; set; }
        public List<Comment>? Comments { get; set; }
        [Required]
        public Guid SourceId { get; set; }
        public Source Source { get; set; }
        public DateTime? Created { get; set; }
        public int? LikesCount { get; set; }
    }
}
