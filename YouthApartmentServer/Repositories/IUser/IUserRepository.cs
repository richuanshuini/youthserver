using YouthApartmentServer.Model.UserPermissionModel;

namespace YouthApartmentServer.Repositories.IUser
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int userId);
        Task<List<User>> GetAllAsync();
        Task<User> InsertAsync(User user);
        Task<User?> GetByUsernameAsync(string username);
    }
}