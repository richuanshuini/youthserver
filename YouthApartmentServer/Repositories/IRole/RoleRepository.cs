using FreeSql;
using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.ModelDto;

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

    public async Task<bool> UpdateAsync(int roleId, UpdateRoleDto patchRoleDto)
    {
        var role =await GetByIdAsync(roleId);
        //freesql中存在update.set，可以设置仅更新的数据
        if (role == null)
            return false;

        var updateRole = base.UpdateDiy.Where(r=>r.RoleId==roleId);
        if(patchRoleDto.RoleName!= null)
            updateRole.Set(r=>r.RoleName,patchRoleDto.RoleName);
        if (patchRoleDto.Description != null)
            updateRole.Set(r => r.Description, patchRoleDto.Description);

        var affectedRows = await updateRole.ExecuteAffrowsAsync();
        return affectedRows > 0;
    }
    public async Task<List<Role>> GetByIdsAsync(IEnumerable<int> ids)
    {
        var idList = ids?.ToList() ?? new List<int>();
        if (idList.Count == 0) return new List<Role>();
        return await Select.Where(r => idList.Contains(r.RoleId)).ToListAsync();
    }

    public async Task<bool> VaildRoleIds(IEnumerable<int> roleIds)
    {
       //处理枚举对象
       var ids = roleIds?.Distinct().ToArray();
       //如果长度为0，则说明不存在需要验证的ids
       if(ids!.Length == 0) return true;
       
       //通过contains验证
       var existCount = await Select.Where(r => ids.Contains(r.RoleId)).CountAsync();
       return existCount == ids.Length;
    }
}