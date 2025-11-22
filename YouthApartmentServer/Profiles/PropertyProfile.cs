using YouthApartmentServer.Model.PropertyManagementModel;
using YouthApartmentServer.ModelDto;

namespace YouthApartmentServer.Profiles;

public class PropertyProfile
{
    public static void Register(Mapster.TypeAdapterConfig config)
    {
        config.NewConfig<Property, PropertyDto>()
            .Map(dest => dest.RealName, src => src.User!.RealName);//新增映射到RealName
        config.NewConfig<Property, InserPropertyDto>();
    }
}