using FreeSql;
using YouthApartmentServer.Model.UserPermissionModel;

namespace YouthApartmentServer.Repositories.IPermission;

public class PermissionRepository:BaseRepository<Permission,int>,IPermissionRepository
{
    public PermissionRepository(IFreeSql fsql) : base(fsql) { }
    public async Task<List<Permission>> GetAllAsync()
    {
        return await Select.ToListAsync();
    }

    public async Task<Permission?> GetByIdAsync(int id)
    {
        return await Select.Where(p=>p.PermissionId==id).ToOneAsync();
    }

    public async Task<Permission> InsertAsync(Permission permission)
    {
        return await base.InsertAsync(permission);
    }

    public async Task<int> DeleteByIdAsync(int id)
    {
        throw new NotImplementedException();
            
    }
}