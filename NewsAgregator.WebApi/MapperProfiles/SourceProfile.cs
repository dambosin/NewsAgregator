﻿using AutoMapper;
using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Entities;

namespace NewsAgregator.WebApi.MapperProfiles
{
    public class SourceProfile : Profile
    {
        public SourceProfile() 
        {
            CreateMap<Source, SourceDto>();
        }
    }
}
