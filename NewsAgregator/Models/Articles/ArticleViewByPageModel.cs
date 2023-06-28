namespace NewsAgregator.Mvc.Models.Articles
{
    public class ArticleViewByPageModel
    {
        public PageInfoModel PageInfo { get; set; }
        public List<ArticleModel> Articles { get; set; }
    }
}
