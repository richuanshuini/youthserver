using FreeSql;
using YouthApartmentServer.Model.UserPermissionModel;
namespace YouthApartmentServer.Repositories.IUserRole;

public class UserRoleRepository:BaseRepository<UserRole>,IUserRoleRepository
{
    public UserRoleRepository(IFreeSql fsql) : base(fsql) { }
    
    public async Task<List<UserRole>> GetAllUserRoleAsync()
    {
        return await Select.Include(x=>x.User).Include(x=>x.Role).ToListAsync();
    }

    public async Task<UserRole> InsertAsync(UserRole userRole)
    {
        return await base.InsertAsync(userRole);
    }
}