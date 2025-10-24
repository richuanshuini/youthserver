using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.Repositories.IUser;

namespace YouthApartmentServer.Services.IUserServices
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _iuserRepository;

        public UserService(IUserRepository iuserRepository)
        {
            _iuserRepository = iuserRepository;
        }

        public Task<List<User>> GetAllUsersAsync()
        {
            return _iuserRepository.GetAllAsync();
        }

        public Task<User?> GetUserByIdAsync(int id)
        {
            return _iuserRepository.GetByIdAsync(id);
        }

        public async Task<User?> CreateUserAsync(User user)
        {
            // 1. 先查：检查用户名是否存在，这里使用！意思是告诉编译器，这个UserName是不可能为空的，但是他静态编译时，会去找User的string ? UserName，但是DTO严格就决定了UserName不可能为空
            //直接无视这个警告
            var existingUser = await _iuserRepository.GetByUsernameAsync(user.UserName!);
            if (existingUser != null)
            {
                return null;
            }

            //查不到才执行插入
            user.Status = false;
            await _iuserRepository.InsertAsync(user);
            return user;

        }
    }
}