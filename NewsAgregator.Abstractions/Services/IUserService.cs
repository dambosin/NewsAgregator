using NewsAgregator.Core.Dto;

namespace NewsAgregator.Abstractions.Services
{
    public interface IUserService
    {
        UserDto GetUser(string login);
        bool LoginUser(string login, string password);
        string RegisterUser(UserDto user);

    }
}
