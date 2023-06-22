namespace NewsAgregator.WebApi.Requests
{
    public class GetArticlesByPageRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
