using NewsAgregator.Core.Dto;
using System.Security.Claims;

namespace NewsAgregator.Abstractions.Services
{
    public interface IUserService
    {
        UserDto GetUser(string login);
        Task<UserDto> GetUserByIdAsync(Guid id);
        ClaimsIdentity LoginUser(string login, string password);
        Task<Guid> RegisterUser(UserDto user);
        public bool IsLoginAvailiable(string login);
        public bool IsEmailAvailiable(string email);
    }
}
