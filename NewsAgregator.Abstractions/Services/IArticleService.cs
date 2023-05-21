using NewsAgregator.Core.Dto;

namespace NewsAgregator.Abstractions.Services
{
    public interface IArticleService
    {
        Task<int> CountAsync();
        Task<IEnumerable<ArticleDto>> GetArticlesByPageAsync(int page, int pageSize);
        Task<ArticleDto> GetArticleDetailAsync(Guid id);
        Task<Guid> CreateAsync(ArticleCreateDto article);
        Task<int> GetPageAmount(int pageSize);
        Task<int> LoadArticlesFromSources();
        Task<double> RateTextAsync(string text);
    }
}
