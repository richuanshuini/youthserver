using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.ModelDto;

namespace YouthApartmentServer.Profiles;

public class RoleProfile
{
    public static void Register(Mapster.TypeAdapterConfig config)
    {
        //查询：Role -> RoleDto
        config.NewConfig<Role, RoleDto>();
        //插入：InsertRoleDto -> Role
        config.NewConfig<InsertRoleDto, Role>();
    }
}