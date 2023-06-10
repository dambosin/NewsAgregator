using AutoMapper;
using NewsAgregator.Core.Dto;
using NewsAgregator.Mvc.Models.Users;

namespace NewsAgregator.Mvc.MapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile() 
        {
            CreateMap<UserDto, UserModel>();
        }
    }
}
