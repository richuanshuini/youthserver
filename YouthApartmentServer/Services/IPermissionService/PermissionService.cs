using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.Repositories.IPermission;

namespace YouthApartmentServer.Services.IPermissionService;

public class PermissionService:IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;

    public PermissionService(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }
    public async Task<List<Permission>> GetAllPermissionsAsync()
    {
        return await _permissionRepository.GetAllAsync();
    }

    public async Task<Permission?> GetPermissionByIdAsync(int id)
    {
        return await _permissionRepository.GetByIdAsync(id);
    }
    
    public async Task<Permission?> CreatePermissionAsync(Permission permission)
    {
        return await _permissionRepository.InsertAsync(permission);
    }

    public async Task<int> DeletePermissionAsync(int id)
    {
        return await _permissionRepository.DeleteByIdAsync(id);
    }
}

    