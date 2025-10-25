using YouthApartmentServer.Model.UserPermissionModel;

namespace YouthApartmentServer.Repositories.IUserRole;

public interface IUserRoleRepository
{
    Task<List<UserRole>> GetAllUserRoleasync();
}