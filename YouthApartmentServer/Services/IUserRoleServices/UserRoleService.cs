using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.Repositories.IRole;
using YouthApartmentServer.Repositories.IUser;
using YouthApartmentServer.Repositories.IUserRole;


namespace YouthApartmentServer.Services.IUserRoleServices;

public class UserRoleService:IUserRoleService
{
    private readonly IUserRoleRepository _iuserRoleRepository;
    private readonly IUserRepository _iuserRepository;
    private readonly IRoleRepository _iroleRepository;

    public UserRoleService(IUserRoleRepository iuserRoleRepository, IUserRepository iuserRepository, IRoleRepository iroleRepository)
    {
        _iuserRoleRepository = iuserRoleRepository;
        _iuserRepository = iuserRepository;
        _iroleRepository = iroleRepository;
    }
    
    public async Task<List<UserRole>> GetAllUserRoleAsync()
    {
        return await _iuserRoleRepository.GetAllAsync();
    }

    public async Task<UserRole?> CreateUserRoleAsync(UserRole userRole)
    {
        //检查是否存在重复数据
        var existUserRole=await _iuserRoleRepository.GetByIdAsync(userRole.UserId,userRole.RoleId);
        if (existUserRole != null)
        {
            //如果存在，则返回null
            return null;
        }
        
        //检查，该User和Role是否存在
        var existUser=await _iuserRepository.GetByIdAsync(userRole.UserId);
        var existRole=await _iroleRepository.GetByIdAsync(userRole.RoleId);
        if (existUser == null||existRole == null)
        {
            return null;
        }
        
        return await _iuserRoleRepository.InsertAsync(userRole);
    }

    public async Task<int> BatchAssignUserRolesAsync(List<int> userIds, List<int> roleIds)
    {
        var validUserIds = new HashSet<int>((await _iuserRepository.GetByIdsAsync(userIds)).Select(u => u.UserId));
        var validRoleIds = new HashSet<int>((await _iroleRepository.GetByIdsAsync(roleIds)).Select(r => r.RoleId));
        if (validUserIds.Count == 0 || validRoleIds.Count == 0) return 0;

        var existing = await _iuserRoleRepository.GetByUserIdsAndRoleIdsAsync(validUserIds.ToList(), validRoleIds.ToList());
        var existingSet = new HashSet<(int uid, int rid)>(existing.Select(e => (e.UserId, e.RoleId)));

        var created = 0;
        foreach (var uid in validUserIds)
        {
            foreach (var rid in validRoleIds)
            {
                var key = (uid, rid);
                if (existingSet.Contains(key)) continue;
                await _iuserRoleRepository.InsertAsync(new UserRole { UserId = uid, RoleId = rid });
                created++;
            }
        }
        return created;
    }
}