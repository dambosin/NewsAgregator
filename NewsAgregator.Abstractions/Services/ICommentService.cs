using NewsAgregator.Core.Dto;

namespace NewsAgregator.Abstractions.Services
{
    public interface ICommentService
    {
        public IQueryable<CommentDto> GetCommentsByArticleId(Guid articleId);
    }
}
