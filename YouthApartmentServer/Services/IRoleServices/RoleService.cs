using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.Repositories.IRole;

namespace YouthApartmentServer.Services.IRoleServices;

public class RoleService:IRoleService
{
    private readonly IRoleRepository _iroleRepository;

    public RoleService(IRoleRepository iroleRepository)
    {
        _iroleRepository = iroleRepository;
    }
    public async Task<List<Role>> GetAllRolesAsync()
    {
        return await _iroleRepository.GetAllAsync();
    }

    public async Task<Role?> GetRoleByIdAsync(int id)
    {
        return await _iroleRepository.GetByIdAsync(id);
    }

    public async Task<Role?> CreateRoleAsync(Role role)
    {
        return await _iroleRepository.InsertAsync(role);
    }
}