using FreeSql;
using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.Repositories.IRole;
using YouthApartmentServer.Repositories.IUser;
using YouthApartmentServer.Repositories.IUserRole;


namespace YouthApartmentServer.Services.IUserRoleService;

public class UserRoleService : IUserRoleService
{
    private readonly IUserRoleRepository _iuserRoleRepository;
    private readonly IUserRepository _iuserRepository;
    private readonly IRoleRepository _iroleRepository;
    private readonly UnitOfWorkManager _uowManager;

    public UserRoleService(
        IUserRoleRepository iuserRoleRepository,
        IUserRepository iuserRepository,
        IRoleRepository iroleRepository,
        UnitOfWorkManager uowManager)
    {
        _iuserRoleRepository = iuserRoleRepository;
        _iuserRepository = iuserRepository;
        _iroleRepository = iroleRepository;
        _uowManager = uowManager;
    }
    
    public async Task<List<UserRole>> GetAllUserRoleAsync()
    {
        return await _iuserRoleRepository.GetAllAsync();
    }

    public async Task<UserRole?> CreateUserRoleAsync(UserRole userRole)
    {
        // 检查是否存在重复数据
        var existUserRole = await _iuserRoleRepository.GetByIdAsync(userRole.UserId, userRole.RoleId);
        if (existUserRole != null)
        {
            // 如果存在，则返回 null
            return null;
        }
        
        // 检查 User 与 Role 是否存在
        var existUser = await _iuserRepository.GetByIdAsync(userRole.UserId);
        var existRole = await _iroleRepository.GetByIdAsync(userRole.RoleId);
        if (existUser == null || existRole == null)
        {
            return null;
        }
        
        return await _iuserRoleRepository.InsertAsync(userRole);
    }

    public async Task<int> BatchAssignUserRolesAsync(List<int> userIds, List<int> roleIds)
    {
        var validUserIds = new HashSet<int>((await _iuserRepository.GetByIdsAsync(userIds)).Select(u => u.UserId));
        var validRoleIds = new HashSet<int>((await _iroleRepository.GetByIdsAsync(roleIds)).Select(r => r.RoleId));
        if (validUserIds.Count == 0 || validRoleIds.Count == 0)
        {
            return 0;
        }

        var existing = await _iuserRoleRepository.GetByUserIdsAndRoleIdsAsync(validUserIds.ToList(), validRoleIds.ToList());
        var existingSet = new HashSet<(int uid, int rid)>(existing.Select(e => (e.UserId, e.RoleId)));

        var created = 0;
        foreach (var uid in validUserIds)
        {
            foreach (var rid in validRoleIds)
            {
                var key = (uid, rid);
                if (existingSet.Contains(key))
                {
                    continue;
                }
                await _iuserRoleRepository.InsertAsync(new UserRole { UserId = uid, RoleId = rid });
                created++;
            }
        }
        return created;
    }

    // 服务层封装事务：一个 userId 可以一次性更新多个 roleId
    public async Task<bool> UpdateUserRolesAsync(int userId, List<int> roleIds)
    {
        var user = await _iuserRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        var distinctRoleIds = roleIds?.Distinct().ToList();
        if (distinctRoleIds == null)
        {
            return false;
        }
        
        //VaildRoleIds内置了当不传入任何roleid时，返回的也是ture，为的就是清空user的role情况
        var validRoleIds = await _iroleRepository.VaildRoleIds(distinctRoleIds);
        if (!validRoleIds)
        {
            return false;
        }
        
        using var uow = _uowManager.Begin();
        try
        {
            await _iuserRoleRepository.DeleteByUserIdAsync(userId);
            //为了实现可以清空user的所有role，所以说要先删除，当count>0时才可以构造新的

            if (distinctRoleIds.Count > 0)
            {
                var newRelations = distinctRoleIds.Select(roleId => new UserRole
                {
                    UserId = userId,
                    RoleId = roleId
                });
                await _iuserRoleRepository.InsertRangeAsync(newRelations);
            }
            uow.Commit();
            return true;
        }
        catch
        {
            uow.Rollback();
            return false;
        }
    }
}
