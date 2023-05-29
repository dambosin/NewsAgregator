using NewsAgregator.Core.Dto;
using System.Security.Claims;

namespace NewsAgregator.Abstractions.Services
{
    public interface IUserService
    {
        UserDto GetUser(string login);
        Task<UserDto> GetUserByIdAsync(Guid id);
        Task<ClaimsIdentity> LoginUserAsync(string login, string password);
        Task<Guid> RegisterUserAsync(UserDto user);
        bool IsLoginAvailiable(string login);
        bool IsEmailAvailiable(string email);
    }
}
