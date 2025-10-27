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
        
        //部分更新
        public async Task<bool> UpdateAsync(int userId, UpdateUserDto updateUserDto)
        {
            //查询是否存在该用户
            var user = await GetByIdAsync(userId);
            if(user==null)
                return false;
            
            //用UpdateDiy查询要更新的对象
            var updateUser =  UpdateDiy.Where(u=>u.UserId==userId);
            
            //将前端传入的DTO，应用于部分更新
            if(updateUserDto.UserName!=null)
                updateUser.Set(u=>u.UserName,updateUserDto.UserName);
            if(updateUserDto.Password!=null)
                updateUser.Set(u=>u.Password,updateUserDto.Password);
            if(updateUserDto.Email!=null)
                updateUser.Set(u=>u.Email,updateUserDto.Email);
            if(updateUserDto.Phone!=null)
                updateUser.Set(u=>u.Phone,updateUserDto.Phone);
            if(updateUserDto.RealName!=null)
                updateUser.Set(u=>u.RealName,updateUserDto.RealName);
            if(updateUserDto.IdCard!=null)
                updateUser.Set(u=>u.IdCard,updateUserDto.IdCard);
            if(updateUserDto.Gender!=null)
                updateUser.Set(u=>u.Gender,updateUserDto.Gender);
            if(updateUserDto.UserAvatarUrl!=null)
                updateUser.Set(u=>u.UserAvatarUrl,updateUserDto.UserAvatarUrl);
            
            //状态就两个，直接更新就行了
            updateUser.Set(u=>u.Status,updateUserDto.Status);

            var affectedRows = await updateUser.ExecuteAffrowsAsync();
            return affectedRows > 0;
        }

        public async Task<int> ExistUserName(string username)
        {
            var user=await Select.Where(u=>u.UserName==username).ToOneAsync();
            //??是空并运行符，当左边为空时，则返回右边的0，当左边不为空时，则返回左边的值
            return user?.UserId ?? 0;
        }
    }
}