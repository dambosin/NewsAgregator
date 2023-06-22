using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Cqs.Queris.Article;

namespace NewsAgregator.Data.Cqs.QueriesHandlers.Article
{
    public class GetArticlesByPageQueryHandler : IRequestHandler<GetArticlesByPageQuery, List<ArticleDto>>
    {
        private readonly NewsAgregatorContext _context;
        private readonly IMapper _mapper;

        public GetArticlesByPageQueryHandler(
            NewsAgregatorContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ArticleDto>> Handle(GetArticlesByPageQuery request, CancellationToken cancellationToken)
        {
            return await _context.Articles
                .AsNoTracking()
                .OrderByDescending(article => article.Created)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(article => _mapper.Map<ArticleDto>(article))
                .ToListAsync(cancellationToken);
        }
    }
}
