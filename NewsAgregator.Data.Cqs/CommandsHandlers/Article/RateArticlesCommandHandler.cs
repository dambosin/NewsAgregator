using MediatR;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Data.Cqs.Commands.Article;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAgregator.Data.Cqs.CommandsHandlers.Article
{
    public class RateArticlesCommandHandler : IRequestHandler<RateArticlesCommand>
    {
        private readonly NewsAgregatorContext _context;
        private readonly IRateService _rateService;

        public RateArticlesCommandHandler(NewsAgregatorContext context, IRateService rateService)
        {
            _context = context;
            _rateService = rateService;
        }

        public async Task Handle(RateArticlesCommand request, CancellationToken cancellationToken)
        {
            var articlesToRate = _context.Articles.Where(article => article.PositiveIndex == -10).ToList();
            foreach (var article in articlesToRate)
            {
                article.PositiveIndex = await _rateService.Rate(article.PlainText);
            }
            _context.UpdateRange(articlesToRate);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
