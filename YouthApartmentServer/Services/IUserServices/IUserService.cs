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
        Task<int> ExistIdCard(string idCard);
        Task<List<User>> SearchUserByContain(UserQueryParams userQueryParams); // 新增：多条件 OR 查询，只能单条件查询，存在多个字段查询时，按优先级查询，且只能查询一个条件
    }
}