using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.ModelDto;

namespace YouthApartmentServer.Profiles;

public class UserRoleProfile
{
    public static void Register(Mapster.TypeAdapterConfig config)
    {
        config.NewConfig<UserRole, UserRoleDto>();
    }
}