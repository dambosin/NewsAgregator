using NewsAgregator.Core.Dto;
using System.Security.Claims;

namespace NewsAgregator.Abstractions.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Get user by login
        /// </summary>
        /// <param name="login">Login of user</param>
        /// <returns>UserDto</returns>
        UserDto GetUserByLogin(string login);
        /// <summary>
        /// Get user by Id
        /// </summary>
        /// <param name="id">Id of user</param>
        /// <returns>Task with UserDto</returns>
        Task<UserDto> GetUserByIdAsync(Guid id);
        /// <summary>
        /// LogIn user
        /// </summary>
        /// <param name="login">Login of user</param>
        /// <param name="password">Password of user</param>
        /// <returns>ClaimsIdentity to authorize user</returns>
        Task<ClaimsIdentity> LoginUser(string login, string password);
        /// <summary>
        /// Post user to database
        /// </summary>
        /// <param name="user">User to post</param>
        /// <returns>Id of posted user</returns>
        Task<Guid> RegisterUserAsync(UserDto user);
        /// <summary>
        /// Check if user exist by his login
        /// </summary>
        /// <param name="login">Login of user</param>
        /// <returns>True if exist</returns>
        bool IsUserExistByLogin(string login);
        /// <summary>
        /// Check if user exist by his email
        /// </summary>
        /// <param name="email">Email of user</param>
        /// <returns>True if exist</returns>
        bool IsUserExistByEmail(string email);
        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List collection of users</returns>
        List<UserDto> GetUsers();
        /// <summary>
        /// Check if password correct
        /// </summary>
        /// <param name="password">Password to check</param>
        /// <param name="passwordHash">Password hash to confirm</param>
        /// <returns>True if correct</returns>
        bool IsPasswordCorrect(string password, string passwordHash);
        Task DeleteAsync(string login);
    }
}
