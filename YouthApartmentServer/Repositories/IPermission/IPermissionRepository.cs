using YouthApartmentServer.Model.UserPermissionModel;

namespace YouthApartmentServer.Repositories.IPermission;

public interface IPermissionRepository
{
    Task<List<Permission>> GetAllAsync();
    Task<Permission?> GetByIdAsync(int id);
    Task<Permission> InsertAsync(Permission permission);
    Task<int> DeleteByIdAsync(int id);
    
}