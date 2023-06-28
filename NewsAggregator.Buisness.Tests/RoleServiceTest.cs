using AutoMapper;
using Moq;
using NewsAgregator.Abstractions.Repository;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Buisness.Services;
using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Entities;
using System.Linq.Expressions;

namespace NewsAggregator.Buisness.Tests
{
    public class RoleServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly Mock<IMapper> _mapper = new();

        private IRoleService CreateRoleService()
        {
            return new RoleService(
                _unitOfWork.Object,
                _mapper.Object);
        }
        [Fact]
        public async Task CreateAsync_CorrectData_CorrectResponse()
        {
            //arrange
            var roles = new List<Role>();

            _unitOfWork.Setup(uow
                => uow.Roles.AddAsync(It.IsAny<Role>()))
                .Callback(() => roles.Add(new Role()));

            _unitOfWork.Setup(uow
                => uow.CommitAsync())
                .ReturnsAsync(1);

            _mapper.Setup(mapper
                => mapper.Map<Role>(It.IsAny<RoleDto>()))
                .Returns(new Role());

            var roleService = CreateRoleService();
            //act
            await roleService.CreateAsync(new RoleDto());
            //assert
            Assert.Single(roles);
        }
        [Fact]
        public void GetRoles_CorrectData_CorrectResponse()
        {
            //arrange
            _unitOfWork.Setup(uow
                => uow.Roles.GetAsQueryable())
                .Returns((new List<Role>
                {
                    new Role(),
                    new Role(),
                    new Role(),
                    new Role(),
                    new Role()
                }).AsQueryable());

            _mapper.Setup(mapper
                => mapper.Map<RoleDto>(It.IsAny<Role>()))
                .Returns(new RoleDto());

            var roleService = CreateRoleService();

            //act
            var result = roleService.GetRoles();
            //assert
            Assert.Equal(5, result.Count);
        }
        [Fact]
        public async Task GetByIdAsync_CorrectData_CorrectResponse()
        {
            //arrange
            _unitOfWork.Setup(uow
                => uow.Roles.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Role());
            _mapper.Setup(mapper
                => mapper.Map<RoleDto>(It.IsAny<Role>()))
                .Returns(new RoleDto());

            var roleService = CreateRoleService();
            //act
            var result = await roleService.GetByIdAsync(Guid.Empty);
            //assert
            Assert.NotNull(result);
        }
        [Fact]
        public async Task GetByIdAsync_WrongData_ThrowArgumentException()
        {
            //arrange
            _unitOfWork.Setup(uow
                => uow.Roles.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Role)null);

            var roleService = CreateRoleService();
            //act & assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await roleService.GetByIdAsync(Guid.Empty));
        }
        [Fact]
        public async Task UpdateAsync_SameName_CorrectResponse()
        {
            //arrange
            const string startName = "Before update";
            const string endName = "After update";
            var guid1 = new Guid("e0377020-cae9-44d1-9fe9-c67a9a1ae615");
            var role = new Role()
            {
                Id = guid1,
                Name = startName
            };

            _unitOfWork.Setup(uow
                => uow.Roles.FindBy(
                    It.IsAny<Expression<Func<Role, bool>>>()))
                .Returns((new List<Role>()
                {
                    role
                }).AsQueryable());

            _unitOfWork.Setup(uow
                => uow.Roles.Update(It.IsAny<Role>()))
                .Callback(() 
                    => role.Name = endName);

            _unitOfWork.Setup(uow
                => uow.CommitAsync())
                .ReturnsAsync(1);

            _mapper.Setup(mapper
                => mapper.Map<Role>(It.IsAny<RoleDto>()))
                .Returns(new Role());

            var roleService = CreateRoleService();
            //act
            await roleService.UpdateAsync(new RoleDto()
            {
                Id = guid1
            });
            //assert
            Assert.Equal(endName, role.Name);
        }
        [Fact]
        public async Task UpdateAsync_DifferentName_CorrectResponse()
        {
            //arrange
            const string startName = "Before update";
            const string endName = "After update";
            var role = new Role()
            {
                Name = startName
            };

            _unitOfWork.Setup(uow
                => uow.Roles.FindBy(
                    It.IsAny<Expression<Func<Role, bool>>>()))
                .Returns((new List<Role>()).AsQueryable());

            _unitOfWork.Setup(uow
                => uow.Roles.Update(It.IsAny<Role>()))
                .Callback(()
                    => role.Name = endName);

            _unitOfWork.Setup(uow
                => uow.CommitAsync())
                .ReturnsAsync(1);

            _mapper.Setup(mapper
                => mapper.Map<Role>(It.IsAny<RoleDto>()))
                .Returns(new Role());

            var roleService = CreateRoleService();
            //act
            await roleService.UpdateAsync(new RoleDto());
            //assert
            Assert.Equal(endName, role.Name);
        }
        [Fact]
        public async Task UpdateAsync_SameNameDifferentId_ThrowsInvalidDataException()
        {
            //arrange
            var guid1 = new Guid("e0377020-cae9-44d1-9fe9-c67a9a1ae615");
            var guid2 = new Guid("43f92588-9e1f-4b50-86a1-0d3284feec95");
            var role = new Role()
            {
                Id = guid1
            };

            _unitOfWork.Setup(uow
                => uow.Roles.FindBy(
                    It.IsAny<Expression<Func<Role, bool>>>()))
                .Returns((new List<Role>()
                {
                    role
                }).AsQueryable());

            var roleService = CreateRoleService();

            //act & assert
            await Assert.ThrowsAsync<InvalidDataException>(async ()
                => await roleService.UpdateAsync(new RoleDto()
                {
                    Id = guid2
                }));
        }
        [Fact]
        public async Task DeleteAsync_CorrectData_CorrectResponse()
        {
            //arrange
            var roles = new List<Role>()
            {
                new Role(),
                new Role()
            };
            
            _unitOfWork.Setup(uow
                => uow.Roles.RemoveAsync(It.IsAny<Guid>()))
                .Callback(()
                    => roles.RemoveAt(1));

            _unitOfWork.Setup(uow
                => uow.CommitAsync())
                .ReturnsAsync(1);

            var roleService = CreateRoleService();
            //act
            await roleService.DeleteAsync(Guid.Empty);
            //assert
            Assert.Single(roles);
        }
    }
}
