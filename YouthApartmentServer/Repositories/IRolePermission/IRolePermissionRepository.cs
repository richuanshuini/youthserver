using YouthApartmentServer.Model.UserPermissionModel;

namespace YouthApartmentServer.Repositories.IRolePermission;

public interface IRolePermissionRepository
{
    Task<List<RolePermission>> GetAllRolePermissionasync();
    
}