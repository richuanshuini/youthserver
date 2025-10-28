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
        
        public async Task<User> InsertAsync(User user)
        {
            return await base.InsertAsync(user);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await Select.Where(u => u.UserName == username).ToOneAsync();
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
        public async Task<bool> UpdateAsync(int userId, PatchUserDto patchUserDto)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
                return false;

            var updateUser = UpdateDiy.Where(u => u.UserId == userId);

            if (patchUserDto.IsUserNameSet)
                updateUser.Set(u => u.UserName, patchUserDto.UserName);
            if (patchUserDto.IsPasswdSet)
                updateUser.Set(u => u.Password, patchUserDto.Password);
            if (patchUserDto.IsEmailSet)
                updateUser.Set(u => u.Email, patchUserDto.Email);
            if (patchUserDto.IsPhoneSet)
                updateUser.Set(u => u.Phone, patchUserDto.Phone);
            if (patchUserDto.IsRealNameSet)
                updateUser.Set(u => u.RealName, patchUserDto.RealName);
            if (patchUserDto.IsIdCradSet)
                updateUser.Set(u => u.IdCard, patchUserDto.IdCrad); // 注意：DTO 是 IdCrad，实体是 IdCard
            if (patchUserDto.IsGenderSet)
                updateUser.Set(u => u.Gender, patchUserDto.Gender);
            if (patchUserDto.IsAvatarUrlSet)
                updateUser.Set(u => u.UserAvatarUrl, patchUserDto.UserAvatarUrl);
            if (patchUserDto.IsStatusSet)
                updateUser.Set(u => u.Status, patchUserDto.Status);

            var affectedRows = await updateUser.ExecuteAffrowsAsync();
            return affectedRows > 0;
        }

        public async Task<int> ExistUserName(string username)
        {
            var user=await Select.Where(u=>u.UserName==username).ToOneAsync();
            //??是空并运行符，当左边为空时，则返回右边的0，当左边不为空时，则返回左边的值
            return user?.UserId ?? 0;
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