using NewsAgregator.Abstractions;
using NewsAgregator.Core.Dto;

namespace NewsAgregator.Buisness.Parsers
{
    public class DtfParseer : ISiteParser
    {
        public List<ArticleDto> Parse(SourceDto source)
        {
            return new List<ArticleDto>();
        }
    }
}

