using NewsAgregator.Core.Dto;

namespace NewsAgregator.Abstractions.Services
{
    public interface IArticleService
    {
        Task<int> CountAsync();
        Task<ArticleDto> GetFullArticleAsync(Guid id);
        Task<IEnumerable<ArticleDto>> GetArticlesByPage(int page, int pageSize);
    }
}
