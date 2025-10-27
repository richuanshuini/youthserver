using Mapster;
using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.ModelDto;

namespace YouthApartmentServer.Profiles
{
    public static class UserProfile
    {
        public static void Register(Mapster.TypeAdapterConfig config)
        {
            // 查询：User -> UserDto
            config.NewConfig<User, UserDto>()
                .Map(dest => dest.Password, src => src.Password);
            // 插入：InsertUserDto -> User
            config.NewConfig<InsertUserDto,User>();
            //全量更新
            config.NewConfig<User,UpdateUserDto>();
        }
    }
}