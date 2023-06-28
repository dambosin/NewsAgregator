using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Entities;
using System.Linq.Expressions;

namespace NewsAgregator.Abstractions.Services
{
    public interface IArticleService
    {
        /// <summary>
        /// Return total number of articles asynchronously
        /// </summary>
        /// <returns>Int value of total number of articles</returns>
        Task<int> CountAsync();
        /// <summary>
        /// Get articles by pages
        /// </summary>
        /// <param name="page">Number of page</param>
        /// <param name="pageSize">Number of articles on the page</param>
        /// <returns>List collection of ArticleDto's </returns>
        List<ArticleDto> GetByPage(int page, int pageSize);
        /// <summary>
        /// Get full article by it's id asynchronously
        /// </summary>
        /// <param name="id">Id of the article</param>
        /// <returns>Task of ArticleDto</returns>
        Task<ArticleDto> GetArticleAsync(Guid id);
        /// <summary>
        /// Post an article to database
        /// </summary>
        /// <param name="article">Article to post</param>
        /// <returns>Id of posted article</returns>
        Task<Guid> CreateAsync(ArticleDto article);
        /// <summary>
        /// Removes article from database
        /// </summary>
        /// <param name="id">Id of article to remove</param>
        /// <returns>Completed task</returns>
        Task RemoveAsync(Guid id);
        /// <summary>
        /// Updates existing in database article
        /// </summary>
        /// <param name="article">Updated article</param>
        /// <returns>Completed task</returns>
        Task UpdateAsync(ArticleDto article);
        /// <summary>
        /// Loads articles from different websites rss feeds
        /// </summary>
        /// <returns>Number of loaded articles</returns>
        Task<int> LoadFromSourcesAsync();
        Task RateArticlesAsync();
        /// <summary>
        /// Rates article and save rate to database
        /// </summary>
        /// <param name="article">Article to rate</param>
        /// <returns>Completed Task</returns>
        Task<double> Rate(Article article);
        /// <summary>
        /// Rates list collection of articles and save rates to database
        /// </summary>
        /// <param name="articles">Articles to rate</param>
        /// <returns>Completed Task</returns>
        Task<List<Article>> Rate(List<Article> articles);
        /// <summary>
        /// Get filtered articles by pages
        /// </summary>
        /// <param name="page">Number of page</param>
        /// <param name="pageSize">Number of articles on page</param>
        /// <param name="expression">Filter expression</param>
        /// <returns>List collection of ArticleDto</returns>
        List<ArticleDto> GetByPageWithFilter(int page, int pageSize, Expression<Func<Article, bool>> expression);
    }
}
