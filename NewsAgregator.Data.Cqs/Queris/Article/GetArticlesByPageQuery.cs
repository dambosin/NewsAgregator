using MediatR;
using NewsAgregator.Core.Dto;

namespace NewsAgregator.Data.Cqs.Queris.Article
{
    public class GetArticlesByPageQuery : IRequest<List<ArticleDto>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
