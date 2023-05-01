using AutoMapper;
using Microsoft.Extensions.Configuration;
using NewsAgregator.Abstractions.Repository;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Entities;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NewsAgregator.Buisness
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly Serilog.ILogger _logger;

        public UserService(
            IConfiguration config,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            Serilog.ILogger logger)
        {
            _config = config;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public UserDto GetUser(string login)
        {
            if (IsLoginAvailiable(login))
            {
                throw new KeyNotFoundException($"User with login : {login} doesn't exist.");
            }
            var user = _unitOfWork.Users.FindBy(user => user.Login == login).First();
            return _mapper.Map<UserDto>(user);
        }
        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            if (!await IsUserExist(id))
            {
                throw new KeyNotFoundException($"User with guid : {id} doesn't exist.");
            }
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            return _mapper.Map<UserDto>(user);
        }
        public ClaimsIdentity LoginUser(string login, string password)
        {
            const string AuthType = "Application Cookie";
            if (!IsLoginAvailiable(login))
            {
                var passwordHash = GetUser(login).PasswordHash;
                if (IsPasswordCorrect(password, passwordHash))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, login),
                    };

                    var identity = new ClaimsIdentity(
                        claims,
                        AuthType,
                        ClaimsIdentity.DefaultNameClaimType,
                        ClaimsIdentity.DefaultRoleClaimType);
                    return identity;
                }
            }
            throw new InvalidDataException("Invalid login or password");
        }
        public async Task<Guid> RegisterUser(UserDto user)
        {
            if (!IsLoginAvailiable(user.Login))
            {
                throw new Exception($"User with login : {user.Login} already exist.");
            }
            if (!IsEmailAvailiable(user.Email))
            {
                throw new Exception($"User with email : {user.Email} already exist.");
            }
            do
            {
                user.Id = Guid.NewGuid();
            } while(await IsUserExist(user.Id));
            user.PasswordHash = GetPasswordHash(user.Password);
            await _unitOfWork.Users.AddAsync(_mapper.Map<User>(user));
            await _unitOfWork.Commit();
            return user.Id;
            
        }
        public bool IsLoginAvailiable(string login)
        {
            return !_unitOfWork.Users.FindBy(user => user.Login.ToLower() == login.ToLower()).Any();
        }
        public bool IsEmailAvailiable(string email)
        {
            return !_unitOfWork.Users.FindBy(user => user.Email == email).Any();
            
        }

        private string GetPasswordHash(string password)
        {
            var sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                var encoding = Encoding.UTF8;
                var result = hash
                    .ComputeHash(
                        encoding
                            .GetBytes($"{password}{_config["Secrets:Salt"]}"));

                foreach (var b in result)
                {
                    sb.Append(b.ToString("x2"));
                }
            }
            return sb.ToString();
        }
        private async  Task<bool> IsUserExist(Guid id)
        {
            return (await _unitOfWork.Users.GetByIdAsync(id)) != null;
        }
        private bool IsPasswordCorrect(string password, string passwordHash)
        {
            return GetPasswordHash(password).Equals(passwordHash);
        }
    }
}
