using AutoMapper;
using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Entities;
using NewsAgregator.Mvc.Models.Sources;

namespace NewsAgregator.Mvc.MapperProfiles
{
    public class SourceProfile : Profile
    {
        public SourceProfile()
        {
            CreateMap<Source, SourceDto>();
            CreateMap<SourceDto, Source>();
            CreateMap<SourceCreateModel, SourceDto>();
            CreateMap<SourceDto, SourceModel>();
        }
    }
}
