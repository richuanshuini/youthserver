using FreeSql;
using YouthApartmentServer.Model.UserPermissionModel;

namespace YouthApartmentServer.Repositories.IRolePermission;

public class RolePermissionRepository : BaseRepository<RolePermission>, IRolePermissionRepository
{
    public RolePermissionRepository(IFreeSql fsql) : base(fsql)
    {
    }

    public async Task<List<RolePermission>> GetAllRolePermissionasync()
    {
        return await Select.ToListAsync();
    }
}

   