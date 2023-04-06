using NewsAgregator.Core.Dto;

namespace NewsAgregator.Abstractions.Services
{
    public interface IArticleService
    {
        Task<int> CountAsync();
        Task<ArticleDto> GetArticleDetailAsync(Guid id);
        Task<IEnumerable<ArticleDto>> GetArticlesByPageAsync(int page, int pageSize);
        Task CreateAsync(ArticleCreateDto
            article);
    }
}
