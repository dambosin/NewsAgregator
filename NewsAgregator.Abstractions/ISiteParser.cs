using NewsAgregator.Core.Dto;

namespace NewsAgregator.Abstractions
{
    public interface ISiteParser
    {
        List<ArticleDto> Parse(SourceDto source);
    }
}
