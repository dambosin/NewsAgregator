using NewsAgregator.Core.Dto;

namespace NewsAgregator.Abstractions.Services
{
    public interface ISourceService
    {
        public List<SourceDto> GetAvailiableSources();
        public List<SourceWithDescriptionDto> GetSources();
        public Task Create(SourceCreateDto source);
    }
}
