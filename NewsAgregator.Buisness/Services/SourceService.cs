using AutoMapper;
using NewsAgregator.Abstractions.Repository;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Entities;

namespace NewsAgregator.Buisness.Services
{
    public class SourceService : ISourceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SourceService( IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Guid> CreateAsync(SourceDto source)
        {
            await _unitOfWork.Sources.AddAsync(_mapper.Map<Source>(source));
            await _unitOfWork.CommitAsync();
            return source.Id;
        }

        public List<SourceDto> GetSources()
        {
            return _unitOfWork.Sources
                .GetAsQueryable()
                .Select(source =>
                    _mapper.Map<SourceDto>(source))
                .ToList();
        }
    }
}
