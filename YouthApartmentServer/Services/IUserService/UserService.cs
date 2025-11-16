using System;
using System.Text.RegularExpressions;
using Mapster;
using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.ModelDto;
using YouthApartmentServer.Repositories.IUser;

namespace YouthApartmentServer.Services.IUserService;

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

    public async Task<ValidationResult<User>> CreateUserAsync(User user)
    {
        var result = new ValidationResult<User>();

        var existingUser = await _iuserRepository.GetByUsernameAsync(user.UserName!);
        if (existingUser != null)
        {
            result.AddError("该用户名已经存在，请重新输入");
        }

        var existingIdCard = await _iuserRepository.GetByIdCardAsync(user.IdCard!);
        if (existingIdCard != null)
        {
            result.AddError("该身份证号已经存在，请重新输入");
        }

        if (!result.IsValid)
            return result;

        if (!string.IsNullOrEmpty(user.UserAvatarUrl) && IsDataUri(user.UserAvatarUrl))
        {
            try
            {
                var savedUrl = await SaveAvatarFromDataUriAsync(0, user.UserAvatarUrl!);
                user.UserAvatarUrl = savedUrl;
            }
            catch (Exception)
            {
                result.AddError("头像格式不正确，请上传有效的图片");
                return result;
            }
        }

        user.Status = false;
        await _iuserRepository.InsertAsync(user);
        result.Data = user;
        return result;
    }

    public async Task<bool> UpdateUserStausAsync(int id, bool status)
    {
        return await _iuserRepository.UpdateUserStatusAsync(id, status);
    }

    public async Task<ValidationResult<User>> UpdateUserAsync(int id, UpdateUserDto userDto)
    {
        var result = new ValidationResult<User>();

        var existingUser = await _iuserRepository.GetByIdAsync(id);
        if (existingUser == null)
        {
            result.MarkNotFound("该用户不存在");
            return result;
        }

        if (!string.IsNullOrWhiteSpace(userDto.UserName))
        {
            var existed = await _iuserRepository.ExistUserName(userDto.UserName);
            if (existed != 0 && existed != id)
                result.AddError("修改后的用户名已经存在，请重新修改");
        }

        if (!string.IsNullOrWhiteSpace(userDto.IdCard))
        {
            var ownerId = await _iuserRepository.ExistIdCard(userDto.IdCard);
            if (ownerId != 0 && ownerId != id)
                result.AddError("修改后的身份证号已经存在，请重新修改");
        }

        if (!result.IsValid)
            return result;

        userDto.Adapt(existingUser);

        if (!string.IsNullOrEmpty(existingUser.UserAvatarUrl) && IsDataUri(existingUser.UserAvatarUrl))
        {
            try
            {
                var savedUrl = await SaveAvatarFromDataUriAsync(id, existingUser.UserAvatarUrl!);
                existingUser.UserAvatarUrl = savedUrl;
            }
            catch (Exception)
            {
                result.AddError("头像格式不正确，请上传有效的图片");
                return result;
            }
        }

        var updated = await _iuserRepository.UpdateAsync(existingUser);
        result.Data = updated;
        return result;
    }

    public async Task<ValidationResult<bool>> PatchUserAsync(int id, UpdateUserDto userDto)
    {
        var result = new ValidationResult<bool>();

        var existingUser = await _iuserRepository.GetByIdAsync(id);
        if (existingUser == null)
        {
            result.MarkNotFound("该用户不存在");
            return result;
        }

        if (!string.IsNullOrWhiteSpace(userDto.UserName))
        {
            var existed = await _iuserRepository.ExistUserName(userDto.UserName);
            if (existed != 0 && existed != id)
                result.AddError("修改后的用户名已经存在，请重新修改");
        }

        if (!string.IsNullOrWhiteSpace(userDto.IdCard))
        {
            var ownerId = await _iuserRepository.ExistIdCard(userDto.IdCard);
            if (ownerId != 0 && ownerId != id)
                result.AddError("修改后的身份证号已经存在，请重新修改");
        }

        if (!result.IsValid)
            return result;

        if (!string.IsNullOrEmpty(userDto.UserAvatarUrl) && IsDataUri(userDto.UserAvatarUrl))
        {
            try
            {
                var savedUrl = await SaveAvatarFromDataUriAsync(id, userDto.UserAvatarUrl!);
                userDto.UserAvatarUrl = savedUrl;
            }
            catch (Exception)
            {
                result.AddError("头像格式不正确，请上传有效的图片");
                return result;
            }
        }

        var success = await _iuserRepository.UpdateAsync(id, userDto);
        if (!success)
        {
            result.MarkNotFound("该用户不存在");
            return result;
        }

        result.Data = true;
        return result;
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
        var match = Regex.Match(dataUri, "^data:(?<mime>[^;]+);base64,(?<data>.+)$");
        if (!match.Success)
            throw new ArgumentException("Invalid data URI for avatar.");

        var mime = match.Groups["mime"].Value;
        var b64 = match.Groups["data"].Value;
        var bytes = Convert.FromBase64String(b64);

        var ext = GetImageExtensionFromMime(mime);
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var fileName = $"avatar_{timestamp}{ext}";
        var folder = Path.Combine(_env.ContentRootPath, "Resources", "UserPermission", "image");
        Directory.CreateDirectory(folder);
        var fullPath = Path.Combine(folder, fileName);

        await File.WriteAllBytesAsync(fullPath, bytes);

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

    public async Task<int> ExistIdCard(string idCard)
    {
        return await _iuserRepository.ExistIdCard(idCard);
    }

    public async Task<List<User>> SearchUserByContain(UserQueryParams userQueryParams)
    {
        if (!string.IsNullOrWhiteSpace(userQueryParams.UserName))
        {
            return await _iuserRepository.SearchUserNameByContain(userQueryParams.UserName);
        }

        if (!string.IsNullOrWhiteSpace(userQueryParams.RealName))
        {
            return await _iuserRepository.SearchRealNameByContain(userQueryParams.RealName);
        }

        if (!string.IsNullOrWhiteSpace(userQueryParams.Email))
        {
            return await _iuserRepository.SearchEmailByContain(userQueryParams.Email);
        }

        if (!string.IsNullOrWhiteSpace(userQueryParams.Phone))
        {
            return await _iuserRepository.SearchPhoneByContain(userQueryParams.Phone);
        }

        if (!string.IsNullOrWhiteSpace(userQueryParams.Gender))
        {
            return await _iuserRepository.SearchGender(userQueryParams.Gender);
        }

        if (userQueryParams.Status.HasValue)
        {
            return await _iuserRepository.SearchStatus(userQueryParams.Status.Value);
        }

        return await _iuserRepository.GetAllAsync();
    }

    public async Task<List<User>> GetUserWithNoRoleAsync()
    {
        return await _iuserRepository.GetUserWithNoRoleAsync();
    }

    public async Task<PagedResult<User>> GetUsersNoRolesPagedAsync(int pageNumber, int pageSize)
    {
        var (items, total) = await _iuserRepository.GetNoRolePagedAsync(pageNumber, pageSize);
        return new PagedResult<User>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Total = total,
            Items = items
        };
    }
    
}
