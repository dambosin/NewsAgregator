namespace NewsAgregator.Mvc.Models
{
    public class ArticleViewByPageModel
    {
        public PageInfoModel PageInfo{ get; set; }
        public List<ArticleModel> Articles { get; set; }
    }
}
