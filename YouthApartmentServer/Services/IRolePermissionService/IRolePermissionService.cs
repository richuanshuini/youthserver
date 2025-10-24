using YouthApartmentServer.Model.UserPermissionModel;

namespace YouthApartmentServer.Services.IRolePermissionService;

public interface IRolePermissionService
{
    Task<List<RolePermission>> GetAllRolePermissionsAsync();
}