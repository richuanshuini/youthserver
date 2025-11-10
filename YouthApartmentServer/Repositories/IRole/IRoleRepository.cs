using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.ModelDto;

namespace YouthApartmentServer.Repositories.IRole;

public interface IRoleRepository
{
    Task<List<Role>> GetAllAsync();
    
    Task<Role?> GetByIdAsync(int id);
    Task<Role> InsertAsync(Role role);
    Task<bool> UpdateAsync(int roleId, UpdateRoleDto patchRoleDto);
    Task<List<Role>> GetByIdsAsync(IEnumerable<int> ids);
    
    //验证插入的roleId是否合法，同时传入null 和 count=0都是合法的，说明希望清空用户的角色
    Task<bool> VaildRoleIds(IEnumerable<int> roleIds);
    
}