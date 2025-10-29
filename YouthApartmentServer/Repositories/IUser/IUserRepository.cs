using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.ModelDto;

namespace YouthApartmentServer.Repositories.IUser
{
    public interface IUserRepository
    {
        #region 查找
        Task<User?> GetByIdAsync(int userId);
        Task<List<User>> GetAllAsync();
        Task<int> ExistUserName(string username);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdCardAsync(string idCard);
        Task<int> ExistIdCard(string idCard);
        // 分页查询用户，返回记录和总数
        Task<(List<User> Items, long Total)> GetPagedAsync(int pageNumber, int pageSize);
        Task<List<User>> SearchUserNameByContain(string username);
        Task<List<User>> SearchRealNameByContain(string realname);
        Task<List<User>> SearchEmailByContain(string email);
        Task<List<User>> SearchPhoneByContain(string phone);
        Task<List<User>> SearchGender(string gender);
        Task<List<User>> SearchStatus(bool status);
        #endregion

        #region 修改
        Task<User> InsertAsync(User user);
        Task<bool> UpdateUserStatusAsync(int id, bool status);
        Task<User> UpdateAsync(User user);
        Task<bool> UpdateAsync(int userId, UpdateUserDto patchUserDto);
        #endregion
        
    }
}