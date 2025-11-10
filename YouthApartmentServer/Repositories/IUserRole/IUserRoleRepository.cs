using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.ModelDto;

namespace YouthApartmentServer.Repositories.IUserRole;

public interface IUserRoleRepository
{
    Task<List<UserRole>> GetAllAsync();
    Task<UserRole?> GetByIdAsync(int userId,int roleId);
    Task<UserRole> InsertAsync(UserRole userRole);
    Task<List<UserRole>> GetByUserIdsAndRoleIdsAsync(List<int> userIds, List<int> roleIds);
    Task<int> DeleteByUserIdAsync(int userId);
    Task<int> InsertRangeAsync(IEnumerable<UserRole> userRoles);
    
    
}