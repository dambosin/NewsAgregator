using AutoMapper;
using NewsAgregator.Data.Cqs.Commands.Token;
using NewsAgregator.Data.Entities;

namespace NewsAgregator.WebApi.MapperProfiles
{
    public class TokenProfile : Profile
    {
        public TokenProfile() 
        {
            CreateMap<AddRefreshTokenCommand, RefreshToken>();
        }
    }
}
