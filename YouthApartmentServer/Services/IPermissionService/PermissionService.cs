using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.Repositories.IPermission;

namespace YouthApartmentServer.Services.IPermissionService;

public class PermissionService:IPermissionService
{
    private readonly IPermissionRepository _ipermissionRepository;

    public PermissionService(IPermissionRepository ipermissionRepository)
    {
        _ipermissionRepository = ipermissionRepository;
    }
    public async Task<List<Permission>> GetAllPermissionsAsync()
    {
        return await _ipermissionRepository.GetAllAsync();
    }

    public async Task<Permission?> GetPermissionByIdAsync(int id)
    {
        return await _ipermissionRepository.GetByIdAsync(id);
    }
    
    public async Task<Permission?> CreatePermissionAsync(Permission permission)
    {
        return await _ipermissionRepository.InsertAsync(permission);
    }

    public async Task<int> DeletePermissionAsync(int id)
    {
        return await _ipermissionRepository.DeleteByIdAsync(id);
    }
}

    