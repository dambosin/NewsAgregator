using NewsAgregator.Abstractions;
using NewsAgregator.Core.Dto;

namespace NewsAgregator.Buisness.Parsers
{
    public class DtfParseer : ISiteParser
    {
        public List<ArticleCreateDto> Parse(SourceDto source)
        {
            return new List<ArticleCreateDto>();
        }
    }
}

