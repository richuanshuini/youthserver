using YouthApartmentServer.Model.UserPermissionModel;

namespace YouthApartmentServer.Services.IUserRoleServices;

public interface IUserRoleService
{
    Task<List<UserRole>> GetAllUserRoleAsync();
}