using NewsAgregator.Core.Dto;

namespace NewsAgregator.Abstractions.Services
{
    public interface ICommentService
    {
        /// <summary>
        /// Get all comments of this article
        /// </summary>
        /// <param name="articleId">Id of article</param>
        /// <returns></returns>
        List<CommentDto> GetCommentsByArticleId(Guid articleId);
        /// <summary>
        /// Post a comment to database
        /// </summary>
        /// <param name="commentDto">Comment to post</param>
        /// <returns>Id of posted comment</returns>
        Task<Guid> CreateAsync(CommentDto commentDto);
        /// <summary>
        /// Remove comment from database
        /// </summary>
        /// <param name="id">Id of comment to remove</param>
        /// <returns>Completed task</returns>
        Task RemoveAsync(Guid id);
        /// <summary>
        /// Update comment in database
        /// </summary>
        /// <param name="commentDto">Updated comment</param>
        /// <returns>Completed task</returns>
        Task Update(CommentDto commentDto);
    }
}
