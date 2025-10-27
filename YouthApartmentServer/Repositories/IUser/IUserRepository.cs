using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.ModelDto;

namespace YouthApartmentServer.Repositories.IUser
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int userId);
        Task<List<User>> GetAllAsync();
        Task<User> InsertAsync(User user);
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> UpdateUserStatusAsync(int id, bool status);
        Task<User> UpdateAsync(User user);
        Task<bool> UpdateAsync(int userId, UpdateUserDto updateUserDto);
    }
}