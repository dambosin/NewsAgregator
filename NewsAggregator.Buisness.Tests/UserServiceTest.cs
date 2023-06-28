using AutoMapper;
using Microsoft.Extensions.Configuration;
using Moq;
using NewsAgregator.Abstractions.Repository;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Buisness.Services;
using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Entities;
using System.Linq.Expressions;

namespace NewsAggregator.Buisness.Tests
{
    public class UserServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly Mock<IConfiguration> _config = new();

        private IUserService CreateUserService()
        {
            return new UserService(
                _unitOfWork.Object,
                _mapper.Object,
                _config.Object);
        }
        [Fact]
        public void GetUserByLogin_CorrectData_CorrectResponse()
        {
            //arrange
            _unitOfWork.Setup(uow
                => uow.Users.FindBy(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns((new List<User>()
                {
                    new User()
                }).AsQueryable());

            _mapper.Setup(mapper
                => mapper.Map<UserDto>(It.IsAny<User>()))
                .Returns(new UserDto());

            var userService = CreateUserService();
            //act
            var result = userService.GetUserByLogin(It.IsAny<string>());

            //assert
            Assert.NotNull(result);
        }
        [Fact]
        public void GetUserByLogin_WrongLogin_ThrowArgumentException()
        {
            //arrange
            _unitOfWork.Setup(uow
                => uow.Users.FindBy(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns((new List<User>()).AsQueryable());

            var userService = CreateUserService();

            //act & assert
            Assert.Throws<ArgumentException>(() 
                => userService.GetUserByLogin(It.IsAny<string>()));
        }
        [Fact]
        public async Task GetUserById_CorrectData_CorrectResponse()
        {
            //arrange
            _unitOfWork.Setup(uow
                => uow.Users.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User());

            _mapper.Setup(mapper
                => mapper.Map<UserDto>(It.IsAny<User>()))
                .Returns(new UserDto());

            var userService = CreateUserService();
            //act
            var result = await userService.GetUserByIdAsync(Guid.Empty);

            //assert
            Assert.NotNull(result);
        }
        [Fact]
        public async Task GetUserById_WrongId_ThrowArgumentException()
        {
            //arrange
            _unitOfWork.Setup(uow
                => uow.Users.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);

            var userService = CreateUserService();

            //act & assert
            await Assert.ThrowsAsync<ArgumentException>(async()
                => await userService.GetUserByIdAsync(Guid.Empty));
        }
        [Fact]
        public void LoginUser_CorrectData_CorrectResponse()
        {
            //arrange
            const string passHash = "ef797c8118f02dfb649607dd5d3f8c7623048c9c063d532cc95c5ed7a898a64f";
;
            const string pass = "12345678";
            
            _unitOfWork.Setup(uow
                => uow.UserRoles.FindBy(
                    It.IsAny<Expression<Func<UserRole, bool>>>()))
                .Returns(new List<UserRole>()
                {
                    new UserRole(),
                    new UserRole()
                }.AsQueryable());

            _mapper.Setup(mapper
                => mapper.Map<UserRoleDto>(It.IsAny<UserRole>()))
                .Returns(new UserRoleDto()
                {
                    RoleId = Guid.Empty
                });
            _unitOfWork.Setup(uow
                => uow.Roles.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Role());
            _mapper.Setup(mapper
                => mapper.Map<RoleDto>(It.IsAny<Role>()))
                .Returns(new RoleDto()
                {
                    Name = "name"
                });
            _config.Setup(cfg
                => cfg["Secrets:Salt"])
                .Returns("");

            Mock<UserService> userServiceMock = new(() => new UserService(
                _unitOfWork.Object,
                _mapper.Object,
                _config.Object))
            {
                CallBase = true
            };
            userServiceMock.Setup(us
                => us.GetUserByLogin(It.IsAny<string>()))
                .Returns(new UserDto() { PasswordHash = passHash});

            var userService = userServiceMock.Object;
            
            //act
            var result = userService.LoginUser("", pass);

            //assert
            Assert.NotNull(result);
        }
        [Fact]
        public async Task LoginUser_WrongEmail_ThrowArgumentException()
        {
            //arrange
            Mock<UserService> userServiceMock = new(() => new UserService(
                _unitOfWork.Object,
                _mapper.Object,
                _config.Object))
            {
                CallBase = true
            };
            userServiceMock.Setup(us
                => us.GetUserByLogin(It.IsAny<string>()))
                .Returns((UserDto)null);

            var userService = userServiceMock.Object;

            //act & assert
            await Assert.ThrowsAsync<ArgumentException>(() 
                => userService.LoginUser("", ""));
        }
        [Fact]
        public async Task LoginUser_WrongPassword_ThrowArgumentException()
        {
            //arrange
            const string passHash = "ef797c8118f02dfb649607dd5d3f8c7623048c9c063d532cc95c5ed7a898a64f";
            ;
            const string pass = "1";

            _config.Setup(cfg
                => cfg["Secrets:Salt"])
                .Returns("");

            Mock<UserService> userServiceMock = new(() => new UserService(
                _unitOfWork.Object,
                _mapper.Object,
                _config.Object))
            {
                CallBase = true
            };
            userServiceMock.Setup(us
                => us.GetUserByLogin(It.IsAny<string>()))
                .Returns(new UserDto() { PasswordHash = passHash });

            var userService = userServiceMock.Object;

            //act & assert
            await Assert.ThrowsAsync<ArgumentException>(() 
                => userService.LoginUser("", pass));
        }
        [Fact]
        public void GetUsers_CorrectData_CorrectResponse()
        {
            //arrange
            _unitOfWork.Setup(uow
                => uow.Users.GetAsQueryable())
                .Returns(new List<User>()
                {
                    new User(),
                    new User(),
                    new User(),
                    new User(),
                    new User()
                }.AsQueryable());

            _mapper.Setup(mapper
                => mapper.Map<UserDto>(It.IsAny<User>()))
                .Returns(new UserDto());

            var userService = CreateUserService();
            //act
            var result = userService.GetUsers();

            //assert
            Assert.Equal(5, result.Count);
        }
        [Fact]
        public void IsLoginAvailiable_NotExistingLogin_True()
        {
            //arrange
            _unitOfWork.Setup(uow
                => uow.Users.FindBy(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(new List<User>().AsQueryable());

            var userService = CreateUserService();
            //act
            var result = userService.IsUserExistByLogin(It.IsAny<string>());

            //assert
            Assert.False(result);
        }
        [Fact]
        public void IsLoginAvailiable_ExistingLogin_False()
        {
            //arrange
            _unitOfWork.Setup(uow
                => uow.Users.FindBy(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(new List<User>()
                {
                    new User()
                }.AsQueryable());

            var userService = CreateUserService();
            //act
            var result = userService.IsUserExistByLogin(It.IsAny<string>());

            //assert
            Assert.True(result);
        }
        [Fact]
        public void IsEmailAvailiable_NotExistingEmail_True()
        {
            //arrange
            _unitOfWork.Setup(uow
                => uow.Users.FindBy(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(new List<User>().AsQueryable());

            var userService = CreateUserService();
            //act
            var result = userService.IsUserExistByEmail(It.IsAny<string>());

            //assert
            Assert.False(result);
        }
        [Fact]
        public void IsEmailAvailiable_ExistingEmail_False()
        {
            //arrange
            _unitOfWork.Setup(uow
                => uow.Users.FindBy(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(new List<User>()
                {
                        new User()
                }.AsQueryable());

            var userService = CreateUserService();
            //act
            var result = userService.IsUserExistByEmail(It.IsAny<string>());

            //assert
            Assert.True(result);
        }
        [Fact]
        public async Task RegisterUserAsync_CorrectData_CorrectResponse()
        {
            //arrange
            var users = new List<User>();
            var userRoles = new List<UserRole>();
            _config.Setup(cfg
                => cfg["Secrets:Salt"])
                .Returns("");

            _unitOfWork.Setup(uow
                => uow.Roles.FindBy(It.IsAny<Expression<Func<Role, bool>>>()))
                .Returns(new List<Role>()
                {
                    new Role()
                    {
                        Id = Guid.Empty
                    }
                }.AsQueryable());

            _unitOfWork.Setup(uow
                => uow.Users.AddAsync(It.IsAny<User>()))
                .Callback(()
                    => users.Add(new User()));

            _unitOfWork.Setup(uow
                => uow.UserRoles.AddAsync(It.IsAny<UserRole>()))
                .Callback(()
                    => userRoles.Add(new UserRole()));

            _unitOfWork.Setup(uow
                => uow.CommitAsync())
                .ReturnsAsync(1);

            _mapper.Setup(mapper
                => mapper.Map<User>(It.IsAny<UserDto>()))
                .Returns(new User());

            _mapper.Setup(mapper
                => mapper.Map<UserRole>(It.IsAny<UserRoleDto>()))
                .Returns(new UserRole());

            var userServiceMock = new Mock<UserService>(()
                => new(
                    _unitOfWork.Object,
                    _mapper.Object,
                    _config.Object))
            {
                CallBase = true
            };
            
            userServiceMock.Setup(us
                => us.IsUserExistByEmail(It.IsAny<string>()))
                .Returns(false);
            
            userServiceMock.Setup(us
                => us.IsUserExistByLogin(It.IsAny<string>()))
                .Returns(false);

            var userService = userServiceMock.Object;

            //act
            var result =await userService.RegisterUserAsync(
                new UserDto()
                {
                    Id = Guid.Empty,
                    Password = "password"
                });

            //assert
            Assert.Single(users);
            Assert.Single(userRoles);
            Assert.Equal(Guid.Empty, result);
        }
        [Fact]
        public async Task ReegisterUserAsync_ExistingLogin_ThrowInvalidDataException()
        {
            //arrange
            Mock<UserService> mock = new(()
                => new(
                    _unitOfWork.Object,
                    _mapper.Object,
                    _config.Object));
            mock.Setup(moq
                => moq.IsUserExistByLogin(It.IsAny<string>()))
                .Returns(true);

            //act & assert
            await Assert.ThrowsAsync<InvalidDataException>(async ()
                => await mock.Object.RegisterUserAsync(new UserDto()));
        }
        [Fact]
        public async Task ReegisterUserAsync_ExistingEmail_ThrowInvalidDataException()
        {
            //arrange
            Mock<UserService> mock = new(()
                => new(
                    _unitOfWork.Object,
                    _mapper.Object,
                    _config.Object));
            mock.Setup(moq
                => moq.IsUserExistByLogin(It.IsAny<string>()))
                .Returns(false);
            mock.Setup(moq
                => moq.IsUserExistByEmail(It.IsAny<string>()))
                .Returns(true);

            //act & assert
            await Assert.ThrowsAsync<InvalidDataException>(async ()
                => await mock.Object.RegisterUserAsync(new UserDto()));
        }
    }
}
