
using AutoMapper;
using Moq;
using NewsAgregator.Abstractions.Repository;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Buisness.Services;
using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Entities;

namespace NewsAggregator.Buisness.Tests
{
    public class SourceServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly Mock<IMapper> _mapper = new();

        private ISourceService CreateSourceService()
        {
            return new SourceService(
                _unitOfWork.Object,
                _mapper.Object);
        }
        [Fact]
        public async Task CreateAsync_CorrectData_CorrectResponse()
        {
            //arrange
            var sources = new List<Source>();

            _unitOfWork.Setup(uow
                => uow.Sources.AddAsync(It.IsAny<Source>()))
                .Callback(() => sources.Add(new Source()));

            _unitOfWork.Setup(uow
                => uow.CommitAsync())
                .ReturnsAsync(1);

            _mapper.Setup(mapper
                => mapper.Map<Source>(It.IsAny<SourceDto>()))
                .Returns(new Source());

            var roleService = CreateSourceService();
            //act
            await roleService.CreateAsync(new SourceDto());
            //assert
            Assert.Single(sources);
        }
        [Fact]
        public void GetSources_CorrectData_CorrectResponse()
        {
            //arrange
            _unitOfWork.Setup(uow
                => uow.Sources.GetAsQueryable())
                .Returns((new List<Source>()
                {
                    new Source(),
                    new Source(),
                    new Source(),
                    new Source(),
                    new Source()
                }).AsQueryable());

            _mapper.Setup(mapper
                => mapper.Map<SourceDto>(It.IsAny<Source>()));

            var sourceService = CreateSourceService();

            //act
            var result = sourceService.GetSources();

            //assert
            Assert.Equal(5, result.Count);
        }
    }
}
