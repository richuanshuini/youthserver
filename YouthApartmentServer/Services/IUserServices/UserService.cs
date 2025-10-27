using Mapster;
using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.ModelDto;
using YouthApartmentServer.Repositories.IUser;
using Microsoft.AspNetCore.Hosting;
using System.Text.RegularExpressions;
using System.Diagnostics.CodeAnalysis;

namespace YouthApartmentServer.Services.IUserServices
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _iuserRepository;
        private readonly IWebHostEnvironment _env;

        public UserService(IUserRepository iuserRepository, IWebHostEnvironment env)
        {
            _iuserRepository = iuserRepository;
            _env = env;
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
            // 若头像是 data URI，保存到 Resources 并替换为静态 URL
            if (!string.IsNullOrEmpty(user.UserAvatarUrl) && IsDataUri(user.UserAvatarUrl))
            {
                var savedUrl = await SaveAvatarFromDataUriAsync(existingUser?.UserId ?? 0, user.UserAvatarUrl!);
                user.UserAvatarUrl = savedUrl;
            }
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
            // 兼容 data URI 头像：保存文件并替换为静态 URL
            if (!string.IsNullOrEmpty(existingUser.UserAvatarUrl) && IsDataUri(existingUser.UserAvatarUrl))
            {
                var savedUrl = await SaveAvatarFromDataUriAsync(id, existingUser.UserAvatarUrl!);
                existingUser.UserAvatarUrl = savedUrl;
            }
            //全量更新
            return await _iuserRepository.UpdateAsync(existingUser);;
        }

        public async Task<bool> PatchUserAsync(int id, UpdateUserDto userDto)
        {
            // 头像字段兼容 Base64：如果是 data URI，则保存到 Resources 并替换为静态 URL
            if (!string.IsNullOrEmpty(userDto.UserAvatarUrl) && IsDataUri(userDto.UserAvatarUrl))
            {
                var savedUrl = await SaveAvatarFromDataUriAsync(id, userDto.UserAvatarUrl!);
                userDto.UserAvatarUrl = savedUrl;
            }

            // 部分更新
            return await _iuserRepository.UpdateAsync(id, userDto);
        }

        private static bool IsDataUri(string value)
        {
            return value.StartsWith("data:", StringComparison.OrdinalIgnoreCase);
        }

        private static string GetImageExtensionFromMime(string mime)
        {
            return mime switch
            {
                "image/png" => ".png",
                "image/jpeg" => ".jpg",
                "image/svg+xml" => ".svg",
                "image/webp" => ".webp",
                _ => ".bin",
            };
        }

        private async Task<string> SaveAvatarFromDataUriAsync(int userId, string dataUri)
        {
            // 格式：data:<mime>;base64,<payload>
            var match = Regex.Match(dataUri, "^data:(?<mime>[^;]+);base64,(?<data>.+)$");
            if (!match.Success)
                throw new ArgumentException("Invalid data URI for avatar.");

            var mime = match.Groups["mime"].Value;
            var b64 = match.Groups["data"].Value;
            var bytes = Convert.FromBase64String(b64);

            var ext = GetImageExtensionFromMime(mime);
            // 以时间戳命名，避免前端端口导致的相对路径解析问题，同时确保唯一性
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var fileName = $"avatar_{timestamp}{ext}";
            var folder = Path.Combine(_env.ContentRootPath, "Resources", "UserPermission", "image");
            Directory.CreateDirectory(folder);
            var fullPath = Path.Combine(folder, fileName);

            await File.WriteAllBytesAsync(fullPath, bytes);

            // 返回静态资源路径
            return $"/resources/UserPermission/image/{fileName}";
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