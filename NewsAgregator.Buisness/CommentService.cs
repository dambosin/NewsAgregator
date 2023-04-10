using AutoMapper;
using NewsAgregator.Abstractions.Repository;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Entities;

namespace NewsAgregator.Buisness
{
    public class CommentService: ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CommentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IQueryable<CommentDto> GetCommentsByArticleId(Guid articleId)
        {
            return _unitOfWork.Comments.FindBy((comment => comment.ArticleId == articleId), (comment => comment.User)).Select(comment => _mapper.Map<CommentDto>(comment)).AsQueryable();
        }
    }
}
