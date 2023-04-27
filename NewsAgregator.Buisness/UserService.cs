using AutoMapper;
using Microsoft.Extensions.Configuration;
using NewsAgregator.Abstractions.Repository;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Core.Dto;
using System.Security.Cryptography;
using System.Text;

namespace NewsAgregator.Buisness
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public UserService(
            IConfiguration config,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _config = config;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public UserDto GetUser(string login)
        {
            if (!IsUserExist(login))
            {
                throw new KeyNotFoundException($"User with login : {login} doesn't exist.");
            }
            var user = _unitOfWork.Users.FindBy(user => user.Login == login);
            return _mapper.Map<UserDto>(user);
        }


        //TODO: Implement login and register
        public bool LoginUser(string login, string password)
        {
            if(IsUserExist(login))
            {
                var passwordHash = GetUser(login).PasswordHash;
                if(IsPasswordCorrect(password, passwordHash))
                {

                }
            }
            return false;
        }


        public string RegisterUser(UserDto user)
        {
            throw new NotImplementedException();
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
        private bool IsUserExist(string login)
        {
            return _unitOfWork.Users.FindBy((user => user.Login == login)).Any();
        }
        private bool IsPasswordCorrect(string password, string passwordHash)
        {
            return GetPasswordHash(password).Equals(passwordHash);
        }
    }
}
