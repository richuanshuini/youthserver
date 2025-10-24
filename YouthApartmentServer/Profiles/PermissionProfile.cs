using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.ModelDto;

namespace YouthApartmentServer.Profiles;

public class PermissionProfile
{
    public static void Register(Mapster.TypeAdapterConfig config)
    {
        config.NewConfig<Permission, PermissionDto>();
    }
}