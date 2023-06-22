using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NewsAgregator.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAgregator.Data.Cqs.QueriesHandlers.Article
{
    internal class GetArticleByIdQueryHandler : IRequestHandler<GetArticleByIdQuery, ArticleDto>
    {
        private readonly NewsAgregatorContext _context;
        private readonly IMapper _mapper;

        public GetArticleByIdQueryHandler(NewsAgregatorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ArticleDto> Handle(GetArticleByIdQuery request, CancellationToken cancellationToken)
        {
            return _mapper
                .Map<ArticleDto>(await _context.Articles
                .SingleOrDefaultAsync(article => article.Id == request.Id, cancellationToken));

        }
    }
}
