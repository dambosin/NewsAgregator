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
            CreateMap<Source, SourceWithDescriptionDto>();
            CreateMap<SourceCreateDto, Source>();
            CreateMap<SourceCreateModel, SourceCreateDto>();
            CreateMap<SourceWithDescriptionDto, SourceModel>();
        }
    }
}
