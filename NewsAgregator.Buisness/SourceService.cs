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

        public async Task<Guid> Create(SourceDto source)
        {
            do
            {
                source.Id = Guid.NewGuid();
            }while(!await IsSourceExistAsync(source.Id));
            await _unitOfWork.Sources.AddAsync(_mapper.Map<Source>(source));
            await _unitOfWork.Commit();
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

        private async Task<bool> IsSourceExistAsync(Guid id)
        {
            var article = await _unitOfWork.Sources.GetByIdAsync(id);
            return article != null;
        }
    }
}
