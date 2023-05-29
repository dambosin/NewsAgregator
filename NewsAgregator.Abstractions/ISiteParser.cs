using NewsAgregator.Core.Dto;

namespace NewsAgregator.Abstractions
{
    public interface ISiteParser
    {
        List<ArticleCreateDto> Parse(SourceDto source);
    }
}
