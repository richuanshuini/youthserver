using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.ModelDto;

namespace YouthApartmentServer.Services.IUserServices
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        
        Task<User?> CreateUserAsync(User user);
        Task<bool> UpdateUserStausAsync(int id, bool status);
        Task<User?> UpdateUserAsync(int id, UpdateUserDto userDto); // 全量更新
        Task<bool> PatchUserAsync(int id, UpdateUserDto userDto); // 部分更新
        Task<int> ExistUserName(string username);
        Task<PagedResult<User>> GetUsersPagedAsync(int pageNumber, int pageSize);
    }
}