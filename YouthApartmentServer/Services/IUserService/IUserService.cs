using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.ModelDto;

namespace YouthApartmentServer.Services.IUserService;
public interface IUserService
{
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    
    Task<ValidationResult<User>> CreateUserAsync(User user);
    Task<bool> UpdateUserStausAsync(int id, bool status);
    Task<ValidationResult<User>> UpdateUserAsync(int id, UpdateUserDto userDto); // 全量更新
    Task<ValidationResult<bool>>     PatchUserAsync(int id, UpdateUserDto userDto); // 部分更新
    Task<int> ExistUserName(string username);
    Task<PagedResult<User>> GetUsersPagedAsync(int pageNumber, int pageSize);
    Task<int> ExistIdCard(string idCard);
    Task<List<User>> SearchUserByContain(UserQueryParams userQueryParams); // 新增：多条件 OR 查询，只能单条件查询，存在多个字段查询时，按优先级查询，且只能查询一个条件
    //查询没有被分配角色的用户
    Task<List<User>> GetUserWithNoRoleAsync();
    Task<PagedResult<User>> GetUsersNoRolesPagedAsync(int pageNumber, int pageSize);
    // 高级筛选已移除：不再提供未分配角色的多条件分页查询
    Task<PagedResult<User>> GetSelectorListAsync(UserSelectorQueryDto query);
}
