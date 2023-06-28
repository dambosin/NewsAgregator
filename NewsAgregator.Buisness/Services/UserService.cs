using AutoMapper;
using Microsoft.Extensions.Configuration;
using NewsAgregator.Abstractions.Repository;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Entities;
using NewsAgregator.Data.Migrations;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NewsAgregator.Buisness.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public UserService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _config = config;
        }

        public virtual UserDto GetUserByLogin(string login)
        {
            var users = _unitOfWork.Users.FindBy(user => user.Login == login);
            if (!users.Any())
                throw new ArgumentException($"User with login : {login} doesn't exist.");
            return _mapper.Map<UserDto>(users.First());
        }
        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                throw new ArgumentException($"User with guid : {id} doesn't exist.");
            return _mapper.Map<UserDto>(user);
        }
        public async Task<ClaimsIdentity> LoginUser(string login, string password)
        {
            const string AuthType = "Application Cookie";
            var user = GetUserByLogin(login);
            if (user == null)
                throw new ArgumentException($"{login} is not registered.");
            if (!IsPasswordCorrect(password, user.PasswordHash))
                throw new ArgumentException("Invalid password");
            var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, login),
                };
            var userRoles = _unitOfWork.UserRoles
                .FindBy(userRole => userRole.UserId == user.Id)
                .Select(uRole => _mapper.Map<UserRoleDto>(uRole))
                .ToList();
            List<RoleDto> roles = new();
            foreach(var userRole in userRoles)
            {
                roles.Add(_mapper.Map<RoleDto>(await _unitOfWork.Roles.GetByIdAsync(userRole.RoleId)));
            }
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Name));
            }

            var identity = new ClaimsIdentity(
                claims,
                AuthType);
            return identity;
        }
        public async Task<Guid> RegisterUserAsync(UserDto user)
        {
            if (IsUserExistByLogin(user.Login))
                throw new InvalidDataException($"User with login : {user.Login} already exist.");
            if (IsUserExistByEmail(user.Email))
                throw new InvalidDataException($"User with email : {user.Email} already exist.");
            user.PasswordHash = GetPasswordHash(user.Password);
            var newUser = await _unitOfWork.Users.AddAsync(_mapper.Map<User>(user));
            var userRole = new UserRoleDto
            {
                UserId = newUser.Entity.Id,
                RoleId = _unitOfWork.Roles.FindBy(role => role.Name.Equals("User")).First().Id
            };
            await _unitOfWork.UserRoles.AddAsync(_mapper.Map<UserRole>(userRole));
            await _unitOfWork.CommitAsync();
            return user.Id;

        }
        public virtual bool IsUserExistByLogin(string login)
        {
            return _unitOfWork.Users.FindBy(user => user.Login.ToLower() == login.ToLower()).Any();
        }
        public virtual bool IsUserExistByEmail(string email)
        {
            return _unitOfWork.Users.FindBy(user => user.Email == email).Any();

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
        public bool IsPasswordCorrect(string password, string passwordHash)
        {
            return GetPasswordHash(password).Equals(passwordHash);
        }

        public List<UserDto> GetUsers()
        {
            return _unitOfWork.Users.GetAsQueryable().Select(user => _mapper.Map<UserDto>(user)).ToList();
        }

        public async Task DeleteAsync(string login)
        {
            await _unitOfWork.Users.RemoveAsync(GetUserByLogin(login).Id);
            await _unitOfWork.CommitAsync();
        }
    }
}
