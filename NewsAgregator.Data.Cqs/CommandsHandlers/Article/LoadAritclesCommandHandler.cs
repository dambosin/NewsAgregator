using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NewsAgregator.Abstractions;
using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Cqs.Commands.Article;
using NewsAgregator.Data.Entities;
using Serilog;

namespace NewsAgregator.Data.Cqs.CommandsHandlers.Article
{
    public class LoadAritclesCommandHandler : IRequestHandler<LoadArticlesCommand>
    {
        private readonly NewsAgregatorContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly ISiteParserFactory _parserFactory;

        public LoadAritclesCommandHandler(NewsAgregatorContext context, ILogger logger, IMapper mapper, ISiteParserFactory parserFactory)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _parserFactory = parserFactory;
        }

        public async Task Handle(LoadArticlesCommand request, CancellationToken cancellationToken)
        {
            var source = _mapper.Map<SourceDto>(await _context.Sources.FirstOrDefaultAsync(src => src.Name.Equals(request.SourceName), cancellationToken));
            if (source == null) throw new ArgumentException();
            var articlesOriginalSiteId = await _context.Articles
                .Where(article => article.SourceId == source.Id)
                .Select(article => article.IdOnSite)
                .ToListAsync(cancellationToken);
            var articles = _parserFactory
                .GetInstance(source.Name)
                .Parse(source)
                .Where(article 
                    => !articlesOriginalSiteId
                    .Any(id 
                        => id.Equals(article.IdOnSite)))
                .Select(article => _mapper.Map<Entities.Article>(article))
                .ToList();
            await _context.Articles.AddRangeAsync(articles, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}
