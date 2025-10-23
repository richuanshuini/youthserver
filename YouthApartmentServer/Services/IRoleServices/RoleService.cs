using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.Repositories.IRole;

namespace YouthApartmentServer.Services.IRoleServices;

public class RoleService:IRoleService
{
    private readonly IRoleRepository _roleRepository;

    public RoleService(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }
    public async Task<List<Role>> GetAllRolesAsync()
    {
        return await _roleRepository.GetAllAsync();
    }

    public async Task<Role?> GetRoleByIdAsync(int id)
    {
        return await _roleRepository.GetByIdAsync(id);
    }

    public async Task<Role?> CreateRoleAsync(Role role)
    {
        return await _roleRepository.InsertAsync(role);
    }
}