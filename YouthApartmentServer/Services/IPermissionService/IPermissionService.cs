using YouthApartmentServer.Model.UserPermissionModel;

namespace YouthApartmentServer.Services.IPermissionService;

public interface IPermissionService
{
    Task<List<Permission>> GetAllPermissionsAsync();
    Task<Permission?> GetPermissionByIdAsync(int id);
    Task<Permission?> CreatePermissionAsync(Permission permission);
    Task<int> DeletePermissionAsync(int id);
}