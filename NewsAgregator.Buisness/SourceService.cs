using AutoMapper;
using NewsAgregator.Abstractions.Repository;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Core.Dto;

namespace NewsAgregator.Buisness
{
    internal class SourceService : ISourceService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public SourceService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public List<SourceDto> GetAvailiableSources()
        {
            return _unitOfWork.Sources.GetAsQueryable().Select(source => _mapper.Map<SourceDto>(source)).ToList();   
        }
    }
}
