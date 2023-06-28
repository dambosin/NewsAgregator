namespace NewsAgregator.Mvc.Models.Articles
{
    public class ArticleModel
    {
        public Guid Id { get; set; }
        public string UrlHeader { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public double PositiveIndex { get; set; }
        public DateTime Created { get; set; }
    }
}
