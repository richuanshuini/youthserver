using FreeSql;
using YouthApartmentServer.Model.UserPermissionModel;

namespace YouthApartmentServer.Repositories.IRole;

public class RoleRepository:BaseRepository<Role,int>,IRoleRepository
{
    public RoleRepository(IFreeSql fsql) : base(fsql) { }
    
    public async Task<List<Role>> GetAllAsync()
    {
        return await Select.ToListAsync();
    }

    public async Task<Role?> GetByIdAsync(int id)
    {
        return await Select.Where(r=>r.RoleId==id).ToOneAsync();
    }

    public async Task<Role> InsertAsync(Role role)
    {
        return await base.InsertAsync(role);
    }
}