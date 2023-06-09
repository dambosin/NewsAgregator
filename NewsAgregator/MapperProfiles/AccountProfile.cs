﻿using AutoMapper;
using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Entities;
using NewsAgregator.Mvc.Models.Accounts;

namespace NewsAgregator.Mvc.MapperProfiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile() 
        { 
            CreateMap<User, UserDto>();
            CreateMap<RegisterModel, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<RegisterModel, LoginModel>();    
        }
    }
}
