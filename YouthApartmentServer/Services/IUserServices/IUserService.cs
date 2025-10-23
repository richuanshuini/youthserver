using YouthApartmentServer.Model.UserPermissionModel;

namespace YouthApartmentServer.Services.IUserServices
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> CreateUserAsync(User user);
    }
}