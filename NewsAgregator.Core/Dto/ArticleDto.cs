using System.Xml.Linq;

namespace NewsAgregator.Core.Dto
{
    public class ArticleDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? Content { get; set; }
        public double? PositiveIndex { get; set; }
        public Guid SourceId { get; set; }
        public DateTime? Created { get; set; }
        public int LikesCount { get; set; }
    }
}
