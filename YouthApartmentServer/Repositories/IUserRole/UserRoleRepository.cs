using FreeSql;
using YouthApartmentServer.Model.UserPermissionModel;
namespace YouthApartmentServer.Repositories.IUserRole;

public class UserRoleRepository:BaseRepository<UserRole>,IUserRoleRepository
{
    public UserRoleRepository(IFreeSql fsql) : base(fsql) { }
    
    public async Task<List<UserRole>> GetAllAsync()
    {
        return await Select.Include(x=>x.User).Include(x=>x.Role).ToListAsync();
    }

    public async Task<UserRole?> GetByIdAsync(int userId, int roleId)
    {
        return await Select.Where(u=>u.UserId==userId).Where(u=>u.RoleId==roleId).ToOneAsync();
    }

    public async Task<UserRole?> GetByIdAsync(int userId)
    {
        return await Select.Where(u=>u.UserId==userId).ToOneAsync();
    }

    public async Task<UserRole> InsertAsync(UserRole userRole)
    {
        return await base.InsertAsync(userRole);
    }

    public async Task<List<UserRole>> GetByUserIdsAndRoleIdsAsync(List<int> userIds, List<int> roleIds)
    {
        if (userIds.Count == 0 || roleIds.Count == 0)
            return new List<UserRole>();
        return await Select.Where(ur => userIds.Contains(ur.UserId) && roleIds.Contains(ur.RoleId)).ToListAsync();
    }

    public async Task<int> DeleteByUserIdAsync(int userId)
    {
        return await base.DeleteAsync(u=>u.UserId==userId);
    }

    public async Task<int> InsertRangeAsync(IEnumerable<UserRole> userRoles)
    {
        var userRole=await base.InsertAsync(userRoles);
        return userRole.Count;
    }
    
    
}