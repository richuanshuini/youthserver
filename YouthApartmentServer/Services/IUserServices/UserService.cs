using Mapster;
using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.ModelDto;
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

        public async Task<bool> UpdateUserStausAsync(int id, bool status)
        {
            return await _iuserRepository.UpdateUserStatusAsync(id, status);
        }

        public async Task<User?> UpdateUserAsync(int id, UpdateUserDto userDto)
        {
            //服务层，调用储仓的，和前端传入的id处理全量更新
            var existingUser = await _iuserRepository.GetByIdAsync(id);
            if (existingUser == null)
                return null;
            //将DTO的更新，映射到已经加载的实体上
            userDto.Adapt(existingUser);
            //全量更新
            return await _iuserRepository.UpdateAsync(existingUser);;
        }

        public async Task<bool> PatchUserAsync(int id, UpdateUserDto userDto)
        {
            //部分更新，直接调用传入id和更新的DTO即可
            return await _iuserRepository.UpdateAsync(id, userDto);
        }

        public async Task<int> ExistUserName(string username)
        {
            return await _iuserRepository.ExistUserName(username);
        }

        public async Task<PagedResult<User>> GetUsersPagedAsync(int pageNumber, int pageSize)
        {
            var (items, total) = await _iuserRepository.GetPagedAsync(pageNumber, pageSize);
            return new PagedResult<User>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Total = total,
                Items = items
            };
        }
    }
}