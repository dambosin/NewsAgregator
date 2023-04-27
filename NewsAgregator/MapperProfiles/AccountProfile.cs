using AutoMapper;
using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Entities;

namespace NewsAgregator.Mvc.MapperProfiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile() 
        { 
            CreateMap<User, UserDto>();
        }
    }
}
