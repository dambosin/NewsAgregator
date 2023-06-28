using NewsAgregator.Abstractions;
using NewsAgregator.Core.Dto;

namespace NewsAgregator.Buisness.Parsers
{
    public class TutbyParser : ISiteParser
    {
        public List<ArticleDto> Parse(SourceDto source)
        {
            return new List<ArticleDto>();
        }
    }
}
