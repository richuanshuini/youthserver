using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.ModelDto;

namespace YouthApartmentServer.Repositories.IUserRole;

public interface IUserRoleRepository
{
    Task<List<UserRole>> GetAllUserRoleAsync();
    Task<UserRole> InsertAsync(UserRole userRole);
}