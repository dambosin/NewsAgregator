﻿using AutoMapper;
using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Entities;
using NewsAgregator.Mvc.Models;

namespace NewsAgregator.Mvc.MapperProfiles
{
    public class ArticleProfile: Profile
    {
        public ArticleProfile() 
        {
            CreateMap<Article, ArticleDto>();
            CreateMap<ArticleDto, Article>();
            CreateMap<ArticleDto, ArticleModel>();
        }
    }
}
