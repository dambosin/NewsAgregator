using AutoMapper;
using NewsAgregator.Abstractions.Repository;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Entities;

namespace NewsAgregator.Buisness
{
    public class SourceService : ISourceService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public SourceService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task Create(SourceCreateDto source)
        {
            await _unitOfWork.Sources.AddAsync(_mapper.Map<Source>(source));
            _unitOfWork.Commit();
        }

        public List<SourceDto> GetAvailiableSources()
        {
            return _unitOfWork.Sources.GetAsQueryable().Select(source => _mapper.Map<SourceDto>(source)).ToList();   
        }

        public List<SourceWithDescriptionDto> GetSources()
        {
            return _unitOfWork.Sources.GetAsQueryable().Select(source => _mapper.Map<SourceWithDescriptionDto>(source)).ToList();
        }
    }
}
