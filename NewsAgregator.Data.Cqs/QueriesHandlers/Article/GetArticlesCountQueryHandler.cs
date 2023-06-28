using MediatR;
using Microsoft.EntityFrameworkCore;
using NewsAgregator.Data.Cqs.Queris.Article;

namespace NewsAgregator.Data.Cqs.QueriesHandlers.Article
{
    public class GetArticlesCountQueryHandler : IRequestHandler<GetArticlesCountQuery, int>
    {
        private readonly NewsAgregatorContext _context;
        public GetArticlesCountQueryHandler(NewsAgregatorContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(GetArticlesCountQuery request, CancellationToken cancellationToken)
        {
            return await _context.Articles.CountAsync(cancellationToken: cancellationToken);
        }
    }
}
