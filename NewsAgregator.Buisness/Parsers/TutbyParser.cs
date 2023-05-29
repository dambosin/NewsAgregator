using NewsAgregator.Abstractions;
using NewsAgregator.Core.Dto;

namespace NewsAgregator.Buisness.Parsers
{
    public class TutbyParser : ISiteParser
    {
        public List<ArticleCreateDto> Parse(SourceDto source)
        {
            return new List<ArticleCreateDto>();
        }
    }
}
