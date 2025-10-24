using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.Repositories.IRolePermission;

namespace YouthApartmentServer.Services.IRolePermissionService;

public class RolePermissionService : IRolePermissionService
{
    private readonly IRolePermissionRepository _ipermissionRepository;
    public RolePermissionService(IRolePermissionRepository irolePermissionRepository)
    {
        _ipermissionRepository = irolePermissionRepository;   
    }
    
    public async Task<List<RolePermission>> GetAllRolePermissionsAsync()
    {
        return await _ipermissionRepository.GetAllRolePermissionasync();
    }
}