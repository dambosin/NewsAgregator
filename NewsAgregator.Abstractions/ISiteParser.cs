using NewsAgregator.Core.Dto;

namespace NewsAgregator.Abstractions
{
    public interface ISiteParser
    {
        Task<List<ArticleCreateDto>> Parse(SourceDto source);
    }
}
