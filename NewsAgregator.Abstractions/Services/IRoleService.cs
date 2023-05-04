using NewsAgregator.Core.Dto;

namespace NewsAgregator.Abstractions.Services
{
    public interface IRoleService
    {
        List<RoleDto> GetRoles();
        Task<Guid> CreateAsyc(RoleDto role);
        Task<RoleDto?> GetByIdAsync(Guid id);
    }
}
