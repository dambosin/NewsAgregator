using NewsAgregator.Abstractions.Services;
using Hangfire;
using MediatR;
using NewsAgregator.Data.Cqs.Commands.Article;

namespace NewsAgregator.Buisness.Services
{
    public class HangfireService : IHangfireService
    {
        private readonly IMediator _mediator;

        public HangfireService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task Init()
        {
            RecurringJob.AddOrUpdate(
                "LoadArticlesDtf",
                () => LoadFromDtf(),
                "0 0,4,8,12,16,20 * * *");

            RecurringJob.AddOrUpdate(
                "LoadArticlesOnliner",
                () => LoadFromOnliner(),
                "0 * * * *");

            RecurringJob.AddOrUpdate(
                "RateArticles",
                () => RateArticles(),
                "5 * * * *");
            return Task.CompletedTask;
        }
        public async Task LoadFromDtf()
        {
            await _mediator.Send(new LoadArticlesCommand()
            {
                SourceName = "Dtf"
            });
        }
        public async Task LoadFromOnliner()
        {
            await _mediator.Send(new LoadArticlesCommand()
            {
                SourceName = "Onliner"
            });
        }
        public async Task RateArticles()
        {
            await _mediator.Send(new RateArticlesCommand());
        }
    }
}
