using AutoMapper;
using Serilog;
using Microsoft.Extensions.Configuration;
using Moq;
using NewsAgregator.Abstractions;
using NewsAgregator.Abstractions.Repository;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Buisness.Services;
using NewsAgregator.Data.Entities;
using NewsAgregator.Core.Dto;

namespace NewsAggregator.Buisness.Tests
{
    public class ArticleServiceTest
    {

        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly Mock<ILogger> _logger = new();
        private readonly Mock<ISiteParserFactory> _siteParserFactory = new();
        private readonly Mock<IConfiguration> _config = new();
        private readonly Mock<ISourceService> _sourceService = new();
        private readonly Mock<IRateService> _rateService = new();
        private IArticleService CreateArticleService()
        {   
            return new ArticleService(
                _unitOfWork.Object,
                _mapper.Object,
                _logger.Object,
                _siteParserFactory.Object,
                _config.Object,
                _sourceService.Object,
                _rateService.Object);
        }

        [Fact]
        public async Task CountAsync_CorrectData_CorrectCount()
        {
            //arrange
            _unitOfWork.Setup(uow
                => uow.Articles.CountAsync())
                .ReturnsAsync(2);

            var articleService = CreateArticleService();
            
            //act
            var result = await articleService.CountAsync();

            //assert
            Assert.Equal(2, result);
        }

        [Theory]
        [InlineData(1, 20)]
        [InlineData(1, 5)]
        [InlineData(1, 10000)]
        public void GetByPageAsync_CorrectPageWithMoreArticles_CorrectResponse(int pageNumber, int pageSize)
        {
            //arrange
            _unitOfWork.Setup(uow
                => uow.Articles.GetAsQueryable())
                .Returns((new List<Article>
                {
                    new Article(),
                    new Article(),
                    new Article(),
                    new Article(),
                    new Article()
                }).AsQueryable());

            _mapper.Setup(mapper
                => mapper.Map<ArticleDto>(It.IsAny<Article>()))
                .Returns(() => new ArticleDto());
            var articleService = CreateArticleService();

            //act
            var result = articleService.GetByPage(pageNumber, pageSize);

            //assert
            Assert.Equal(5, result.Count());
        }

        [Theory]
        [InlineData(1, 4)]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        public void GetByPageAsync_CorrectPageWithLessArticles_CorrectResponse(int pageNumber, int pageSize)
        {
            //arrange
            _unitOfWork.Setup(uow
                => uow.Articles.GetAsQueryable())
                .Returns((new List<Article>
                {
                    new Article(),
                    new Article(),
                    new Article(),
                    new Article(),
                    new Article()
                }).AsQueryable());

            _mapper.Setup(mapper
                => mapper.Map<ArticleDto>(It.IsAny<Article>()))
                .Returns(() => new ArticleDto());
            var articleService = CreateArticleService();

            //act
            var result = articleService.GetByPage(pageNumber, pageSize);

            //assert
            Assert.Equal(pageSize, result.Count());
        }
        
        [Theory]
        [InlineData(2, 3)]
        [InlineData(3, 2)]
        [InlineData(2, 4)]
        public void GetByPageAsync_LastPageWithLessArticles_CorrectResponse(int pageNumber, int pageSize)
        {
            //arrange
            _unitOfWork.Setup(uow
                => uow.Articles.GetAsQueryable())
                .Returns((new List<Article>
                {
                    new Article(),
                    new Article(),
                    new Article(),
                    new Article(),
                    new Article()
                }).AsQueryable());

            _mapper.Setup(mapper
                => mapper.Map<ArticleDto>(It.IsAny<Article>()))
                .Returns(() => new ArticleDto());
            var articleService = CreateArticleService();

            //act
            var result = articleService.GetByPage(pageNumber, pageSize);

            //assert
            Assert.Equal(5 - (pageNumber - 1) * pageSize, result.Count());
        }

        [Theory]
        [InlineData(0, 5)]
        [InlineData(-10, 5)]
        [InlineData(2, 5)]
        [InlineData(1000, 5)]
        public void GetByPageAsync_WrongPage_ThrowArgumentExtension(int pageNumber, int pageSize)
        {
            //arrange
            _unitOfWork.Setup(uow
                => uow.Articles.GetAsQueryable())
                .Returns((new List<Article>
                {
                    new Article(),
                    new Article(),
                    new Article(),
                    new Article(),
                    new Article()
                }).AsQueryable());

            _mapper.Setup(mapper
                => mapper.Map<ArticleDto>(It.IsAny<Article>()))
                .Returns(() => new ArticleDto());
            var articleService = CreateArticleService();

            //act & assert
            Assert.Throws<ArgumentOutOfRangeException>(() => articleService.GetByPage(pageNumber,pageSize));
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(1, -5)]
        [InlineData(1, -10000)]

        public void GetByPageAsync_WrongSize_ThrowArgumentExtension(int pageNumber, int pageSize)
        {
            //arrange
            _unitOfWork.Setup(uow
                => uow.Articles.GetAsQueryable())
                .Returns((new List<Article>
                {
                    new Article(),
                    new Article(),
                    new Article(),
                    new Article(),
                    new Article()
                }).AsQueryable());

            _mapper.Setup(mapper
                => mapper.Map<ArticleDto>(It.IsAny<Article>()))
                .Returns(() => new ArticleDto());
            var articleService = CreateArticleService();

            //act & assert
            Assert.Throws<ArgumentOutOfRangeException>(() => articleService.GetByPage(pageNumber, pageSize));
        }

        [Fact]
        public async Task GetArticleAsync_CorrectData_CorrectResponse()
        {
            //arrange
            _unitOfWork.Setup(uow
                => uow.Articles.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Article());
            _mapper.Setup(mapper
                => mapper.Map<ArticleDto>(It.IsAny<Article>()))
                .Returns(new ArticleDto());

            var articleService = CreateArticleService();
            //act
            var result = await articleService.GetArticleAsync(Guid.Empty);
            //assert
            Assert.NotNull(result);
        }
        [Fact]
        public async Task GetArticleAsync_WrongData_ThrowArgumentException()
        {
            //arrange
            _unitOfWork.Setup(uow
                => uow.Articles.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Article)null!);

            var articleService = CreateArticleService();
            //act & assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await articleService.GetArticleAsync(Guid.Empty));
        }

        [Fact]
        public async Task CreateAsync_CorrectData_CorrectResponse()
        {
            //arrange
            var articles = new List<Article>();

            _unitOfWork.Setup(uow
                => uow.Articles.AddAsync(It.IsAny<Article>()))
                .Callback(() => articles.Add(new Article()));

            _unitOfWork.Setup(uow
                => uow.CommitAsync())
                .ReturnsAsync(1);

            _mapper.Setup(mapper
                => mapper.Map<Article>(It.IsAny<ArticleDto>()))
                .Returns(new Article());

            var articleService = CreateArticleService();
            //act
            await articleService.CreateAsync(new ArticleDto());
            //assert
            Assert.Single(articles);
        }

        /*[Fact]
        public async Task RemoveAsync_CorrectData_CorrectResponse()
        {
            //arrange
            var articles = new List<ArticleDto>()
            {
                new()
            };
            var _articleService = CreateArticleService();
            //act
            _articleService.RemoveAsync(Guid.Empty);
            //assert
            Assert.Empty(articles);
        }*/
    }
}