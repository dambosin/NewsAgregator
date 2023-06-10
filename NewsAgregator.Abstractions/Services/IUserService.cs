using NewsAgregator.Core.Dto;
using System.Security.Claims;

namespace NewsAgregator.Abstractions.Services
{
    public interface IUserService
    {
        UserDto GetUserByLogin(string login);
        Task<UserDto> GetUserByIdAsync(Guid id);
        ClaimsIdentity LoginUser(string login, string password);
        Task<Guid> RegisterUserAsync(UserDto user);
        bool IsLoginAvailiable(string login);
        bool IsEmailAvailiable(string email);
        List<UserDto> GetUsers();
    }
}
