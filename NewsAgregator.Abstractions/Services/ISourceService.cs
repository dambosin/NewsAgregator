using NewsAgregator.Core.Dto;

namespace NewsAgregator.Abstractions.Services
{
    public interface ISourceService
    {
        public List<SourceDto> GetAvailiableSources();
    }
}
