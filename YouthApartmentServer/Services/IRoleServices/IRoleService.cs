using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.ModelDto;

namespace YouthApartmentServer.Services.IRoleServices;

public interface IRoleService
{
    Task<List<Role>> GetAllRolesAsync();
    Task<Role?> GetRoleByIdAsync(int id);
    Task<Role> CreateRoleAsync(Role role);
    Task<bool> UpdateRoleAsync(int roleId, UpdateRoleDto patchRoleDto);
}