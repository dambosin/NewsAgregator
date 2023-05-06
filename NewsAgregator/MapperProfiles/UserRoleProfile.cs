using AutoMapper;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Entities;

namespace NewsAgregator.Mvc.MapperProfiles
{
    public class UserRoleProfile : Profile
    {
        public UserRoleProfile() 
        {
            CreateMap<UserRoleDto, UserRole>();
            CreateMap<UserRole, UserRoleDto>()
                .ForMember(dto => dto.Role,
                    opt => opt.MapFrom(ent => ent.Role));
        }
    }
}
