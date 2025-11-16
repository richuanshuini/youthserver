using YouthApartmentServer.Model.UserPermissionModel;

namespace YouthApartmentServer.Services.IUserRoleService;

public interface IUserRoleService
{
    Task<List<UserRole>> GetAllUserRoleAsync();
    Task<UserRole?> CreateUserRoleAsync(UserRole userRole);
    Task<int> BatchAssignUserRolesAsync(List<int> userIds, List<int> roleIds);
    Task<bool> UpdateUserRolesAsync(int userId, List<int> roleIds);
    
}