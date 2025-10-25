using FreeSql;
using YouthApartmentServer.Model.UserPermissionModel;
namespace YouthApartmentServer.Repositories.IUserRole;

public class UserRoleRepository:BaseRepository<UserRole>,IUserRoleRepository
{
    public UserRoleRepository(IFreeSql fsql) : base(fsql) { }
    
    public async Task<List<UserRole>> GetAllUserRoleasync()
    {
        return await Select.ToListAsync();
    }
}