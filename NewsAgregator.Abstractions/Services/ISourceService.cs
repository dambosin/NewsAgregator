using NewsAgregator.Core.Dto;

namespace NewsAgregator.Abstractions.Services
{
    public interface ISourceService
    {
        public Task<Guid> CreateAsync(SourceDto source);
        public List<SourceDto> GetSources();
    }
}
