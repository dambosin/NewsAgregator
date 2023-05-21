using AutoMapper;
using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Entities;
using NewsAgregator.Mvc.Models.Roles;

namespace NewsAgregator.Mvc.MapperProfiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile() 
        {
            CreateMap<Role, RoleDto>();
            CreateMap<RoleDto, Role>();
            CreateMap<RoleCreateModel, RoleDto>();
            CreateMap<RoleUpdateModel, RoleDto>();
            CreateMap<RoleDto, RoleUpdateModel>();
        }
    }
}
