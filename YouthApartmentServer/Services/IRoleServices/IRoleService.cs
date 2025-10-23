using YouthApartmentServer.Model.UserPermissionModel;
namespace YouthApartmentServer.Services.IRoleServices;

public interface IRoleService
{
    Task<List<Role>> GetAllRolesAsync();
    Task<Role?> GetRoleByIdAsync(int id);
    Task<Role?> CreateRoleAsync(Role role);
}