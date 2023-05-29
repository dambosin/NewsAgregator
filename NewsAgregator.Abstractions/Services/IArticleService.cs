using NewsAgregator.Core.Dto;

namespace NewsAgregator.Abstractions.Services
{
    public interface IArticleService
    {
        Task<int> CountAsync();
        Task<IEnumerable<ArticleDto>> GetByPageAsync(int page, int pageSize);
        Task<ArticleDto> GetDetailAsync(Guid id);
        Task<Guid> CreateAsync(ArticleCreateDto article);
        Task<int> LoadFromSourcesAsync();
        Task<double> RateTextAsync(string text);
        Task Rate(ArticleDto article);
        Task Rate(List<ArticleDto> articles);

    }
}
