using NewsAgregator.Core.Dto;

namespace NewsAgregator.Mvc.Models.Articles
{
    public class ArticleDetailModel
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? Content { get; set; }
        public double? PositiveIndex { get; set; }
        public List<CommentDto>? Comments { get; set; }
        public DateTime? Created { get; set; }
        public int LikesCount { get; set; }
    }
}
