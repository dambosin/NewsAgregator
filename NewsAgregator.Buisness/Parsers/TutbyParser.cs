using NewsAgregator.Abstractions;
using NewsAgregator.Core.Dto;

namespace NewsAgregator.Buisness.Parsers
{
    public class TutbyParser : ISiteParser
    {
        public Task<List<ArticleCreateDto>> Parse(SourceDto source)
        {
            return Task.FromResult(new List<ArticleCreateDto>());
        }
    }
}
