using NewsAgregator.Core.Dto;

namespace NewsAgregator.Abstractions.Services
{
    public interface IRoleService
    {
        List<RoleDto> GetRoles();
        Task<Guid> CreateAsync(RoleDto role);
        Task<RoleDto?> GetByIdAsync(Guid id);
        Task UpdateAsync(RoleDto role);
        Task DeleteAsync(Guid id);
    }
}
