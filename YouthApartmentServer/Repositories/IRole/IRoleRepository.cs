using YouthApartmentServer.Model.UserPermissionModel;
namespace YouthApartmentServer.Repositories.IRole;

public interface IRoleRepository
{
    Task<List<Role>> GetAllAsync();
    Task<Role?> GetByIdAsync(int id);
    Task<Role> InsertAsync(Role role);
    
    
}