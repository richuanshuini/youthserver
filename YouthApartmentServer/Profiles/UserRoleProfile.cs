using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.ModelDto;

namespace YouthApartmentServer.Profiles;

public class UserRoleProfile
{
    public static void Register(Mapster.TypeAdapterConfig config)
    {
        config.NewConfig<UserRole, UserRoleDto>()
            .Map(dest=>dest.UserName,src=>src.User!.UserName)
            .Map(dest=>dest.RealName,src=>src.User!.RealName)
            .Map(dest=>dest.RoleName,src=>src.Role!.RoleName);
        config.NewConfig<UserRoleDto, UserRole>();
    }
}
