using FreeSql;
using YouthApartmentServer.Model.UserPermissionModel;

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

        public async Task<bool> SetUserStatusAsync(int id, bool status)
        {
            var result = await UpdateDiy.Set(s => s.Status, status).Where(i => i.UserId == id).ExecuteAffrowsAsync();
            return  result > 0;
        }
    }
}