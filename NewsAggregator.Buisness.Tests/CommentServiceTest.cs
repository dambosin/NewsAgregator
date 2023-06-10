using AutoMapper;
using Moq;
using NewsAgregator.Abstractions.Repository;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Buisness.Services;
using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Entities;
using System.Linq.Expressions;

namespace NewsAggregator.Buisness.Tests
{
    public class CommentServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly Mock<IMapper> _mapper = new();

        private ICommentService CreateCommentService()
        {
            return new CommentService(
                _unitOfWork.Object, 
                _mapper.Object);
        }

        [Fact]
        public void GetCommentsByArticleId_CorrectData_CorrectResponse()
        {
            //arrange
            _unitOfWork.Setup(uow
                => uow.Comments.FindBy(
                    It.IsAny<Expression<Func<Comment, bool>>>(), 
                    It.IsAny<Expression<Func<Comment, object>>[]>()))
                .Returns((new List<Comment>()
                {
                    new Comment(),
                    new Comment(),
                    new Comment(),
                    new Comment(),
                    new Comment()
                }).AsQueryable);
            _mapper.Setup(mapper
                => mapper.Map<CommentDto>(It.IsAny<Comment>()))
                .Returns(new CommentDto());

            var commentService = CreateCommentService();

            //act
            var result = commentService.GetCommentsByArticleId(Guid.Empty);

            //assert
            Assert.Equal(5, result.Count());
        }
    }
}
