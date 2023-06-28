using MediatR;

namespace NewsAgregator.Data.Cqs.Commands.Article
{
    public class LoadArticlesCommand : IRequest
    {
        public string SourceName { get; set; }
    }
}
