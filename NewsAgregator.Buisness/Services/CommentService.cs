using AutoMapper;
using NewsAgregator.Abstractions.Repository;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Core.Dto;

namespace NewsAgregator.Buisness.Services
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CommentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<Guid> CreateAsync(CommentDto commentDto)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task Update(CommentDto commentDto)
        {
            throw new NotImplementedException();
        }

        List<CommentDto> ICommentService.GetCommentsByArticleId(Guid articleId)
        {
            throw new NotImplementedException();
        }
    }
}
