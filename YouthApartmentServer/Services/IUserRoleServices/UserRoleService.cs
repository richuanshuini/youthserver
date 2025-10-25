using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.Repositories.IUserRole;

namespace YouthApartmentServer.Services.IUserRoleServices;

public class UserRoleService:IUserRoleService
{
    private readonly IUserRoleRepository _iuserRoleRepository;

    public UserRoleService(IUserRoleRepository iuserRoleRepository)
    {
        _iuserRoleRepository = iuserRoleRepository;
    }
    
    public async Task<List<UserRole>> GetAllUserRoleAsync()
    {
        return await _iuserRoleRepository.GetAllUserRoleasync();
    }
}