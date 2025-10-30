using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.ModelDto;

namespace YouthApartmentServer.Repositories.IRole;

public interface IRoleRepository
{
    Task<List<Role>> GetAllAsync();
    
    Task<Role?> GetByIdAsync(int id);
    Task<Role> InsertAsync(Role role);
    Task<bool> UpdateAsync(int roleId, UpdateRoleDto patchRoleDto);
    
}