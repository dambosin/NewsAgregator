using NewsAgregator.Core.Dto;

namespace NewsAgregator.Abstractions.Services
{
    public interface ISourceService
    {
        //todo: is it suitable in this service of should i move it to adminService
        public Task<Guid> CreateAsync(SourceDto source);
        public List<SourceDto> GetSources();
    }
}
