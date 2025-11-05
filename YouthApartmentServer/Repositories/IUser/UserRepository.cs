using FreeSql;
using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.ModelDto;

namespace YouthApartmentServer.Repositories.IUser
{
    public class UserRepository : BaseRepository<User, int>, IUserRepository
    {
        public UserRepository(IFreeSql fsql) : base(fsql) { }

        public async Task<User?> GetByIdAsync(int userId)
        {
            return await Select.Where(u => u.UserId == userId).ToOneAsync();
        }
        
        public async Task<List<User>> GetAllAsync()
        {
            return await Select.ToListAsync();
        }
            
        //
        public async Task<List<User>> SearchUserNameByContain(string username)
        {
            return await Select.Where(u=>u.UserName!.Contains(username)).ToListAsync()!;
        }

        public async Task<List<User>> SearchRealNameByContain(string realname)
        {
            return await Select.Where(u => u.RealName!.Contains(realname)).ToListAsync();
        }

        public async Task<List<User>> SearchEmailByContain(string email)
        {
            return await Select.Where(u=>u.Email!.Contains(email)).ToListAsync();
        }

        public async Task<List<User>> SearchPhoneByContain(string phone)
        {
            return await Select.Where(u=>u.Phone!.Contains(phone)).ToListAsync();
        }

        public async Task<List<User>> SearchGender(string gender)
        {
            return await Select.Where(u=>u.Gender==gender).ToListAsync();
        }

        public async Task<List<User>> SearchStatus(bool status)
        {
            return await Select.Where(u=>u.Status==status).ToListAsync();
        }

        public async Task<User> InsertAsync(User user)
        {
            return await base.InsertAsync(user);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await Select.Where(u => u.UserName == username).ToOneAsync();
        }

        public async Task<User?> GetByIdCardAsync(string idCard)
        {
            return await Select.Where(u=>u.IdCard==idCard).ToOneAsync();
        }


        public async Task<bool> UpdateUserStatusAsync(int id, bool status)
        {
            var result = await UpdateDiy.Set(s => s.Status, status).Where(i => i.UserId == id).ExecuteAffrowsAsync();
            return  result > 0;
        }

        //全量更新
        public async Task<User> UpdateAsync(User user)
        {
            await base.UpdateAsync(user);
            return user;
        }
        
        //部分更新（新：基于 PatchUserDto 标记位）
        public async Task<bool> UpdateAsync(int userId, UpdateUserDto patchUserDto)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
                return false;

            var updateUser = UpdateDiy.Where(u => u.UserId == userId);
            var changed = false;

            if (patchUserDto.UserName != null &&
                !string.Equals(patchUserDto.UserName, user.UserName, StringComparison.Ordinal))
            {
                updateUser.Set(u => u.UserName, patchUserDto.UserName);
                changed = true;
            }

            if (!string.IsNullOrWhiteSpace(patchUserDto.Password) &&
                !string.Equals(patchUserDto.Password, user.Password, StringComparison.Ordinal))
            {
                updateUser.Set(u => u.Password, patchUserDto.Password);
                changed = true;
            }

            if (patchUserDto.Email != null &&
                !string.Equals(patchUserDto.Email, user.Email, StringComparison.Ordinal))
            {
                updateUser.Set(u => u.Email, patchUserDto.Email);
                changed = true;
            }

            if (patchUserDto.Phone != null &&
                !string.Equals(patchUserDto.Phone, user.Phone, StringComparison.Ordinal))
            {
                updateUser.Set(u => u.Phone, patchUserDto.Phone);
                changed = true;
            }

            if (patchUserDto.RealName != null &&
                !string.Equals(patchUserDto.RealName, user.RealName, StringComparison.Ordinal))
            {
                updateUser.Set(u => u.RealName, patchUserDto.RealName);
                changed = true;
            }

            if (patchUserDto.IdCard != null &&
                !string.Equals(patchUserDto.IdCard, user.IdCard, StringComparison.Ordinal))
            {
                updateUser.Set(u => u.IdCard, patchUserDto.IdCard);
                changed = true;
            }

            if (patchUserDto.Gender != null &&
                !string.Equals(patchUserDto.Gender, user.Gender, StringComparison.Ordinal))
            {
                updateUser.Set(u => u.Gender, patchUserDto.Gender);
                changed = true;
            }

            if (patchUserDto.UserAvatarUrl != null &&
                !string.Equals(patchUserDto.UserAvatarUrl, user.UserAvatarUrl, StringComparison.Ordinal))
            {
                updateUser.Set(u => u.UserAvatarUrl, patchUserDto.UserAvatarUrl);
                changed = true;
            }

            // Status 不在此方法更新，保持与专用状态接口解耦

            // 若无任何字段变化，认为操作成功（用户存在且无需更新）
            if (!changed)
                return true;

            var affectedRows = await updateUser.ExecuteAffrowsAsync();
            return affectedRows > 0;
        }

        public async Task<int> ExistUserName(string username)
        {
            var user = await Select.Where(u => u.UserName == username).ToOneAsync();
            return user?.UserId ?? 0;
        }

        public async Task<int> ExistIdCard(string idCard)
        {
            var user = await Select.Where(u => u.IdCard == idCard).ToOneAsync();
            return user?.UserId ?? 0;
        }

        public async Task<List<User>> GetUserWithNoRoleAsync()
        {
            //freesql可直接通过导航属性加载相关联的表，!.any相当于是不存在这个集合
            return await Select.Where(u=>!u.UserRoles.Any()).ToListAsync();
        }

        public async Task<(List<User> Items, long Total)> GetNoRolePagedAsync(int pageNumber, int pageSize)
        {
            var select = Select.Where(u => !u.UserRoles.Any());
            var total = await select.CountAsync();
            var items = await select.Page(pageNumber, pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<(List<User> Items, long Total)> SearchNoRoleUsersPagedAsync(UserNoRoleSearchParams query)
        {
            var select = Select.Where(u => !u.UserRoles.Any());
            if (!string.IsNullOrWhiteSpace(query.UserName))
                select = select.Where(u => u.UserName!.Contains(query.UserName));
            if (!string.IsNullOrWhiteSpace(query.Email))
                select = select.Where(u => u.Email!.Contains(query.Email));
            if (!string.IsNullOrWhiteSpace(query.Phone))
                select = select.Where(u => u.Phone!.Contains(query.Phone));
            if (!string.IsNullOrWhiteSpace(query.RealName))
                select = select.Where(u => u.RealName!.Contains(query.RealName));
            if (!string.IsNullOrWhiteSpace(query.IdCard))
                select = select.Where(u => u.IdCard!.Contains(query.IdCard));
            if (!string.IsNullOrWhiteSpace(query.Gender))
                select = select.Where(u => u.Gender == query.Gender);

            var total = await select.CountAsync();
            var items = await select.Page(query.PageNumber, query.PageSize).ToListAsync();
            return (items, total);
        }

        public async Task<List<User>> GetByIdsAsync(IEnumerable<int> ids)
        {
            var idList = ids?.ToList() ?? new List<int>();
            if (idList.Count == 0) return new List<User>();
            return await Select.Where(u => idList.Contains(u.UserId)).ToListAsync();
        }

        public async Task<(List<User> Items, long Total)> GetPagedAsync(int pageNumber, int pageSize)
        {
            var select = Select;
            var total = await select.CountAsync();
            var items = await select.Page(pageNumber, pageSize).ToListAsync();
            return (items, total);
        }
        
        
    }
}