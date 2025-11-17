using YouthApartmentServer.Model.PropertyManagementModel;
using YouthApartmentServer.ModelDto;

namespace YouthApartmentServer.Profiles;

public class PropertyProfile
{
    public static void Register(Mapster.TypeAdapterConfig config)
    {
        config.NewConfig<Property, PropertyDto>();
        config.NewConfig<Property, InserPropertyDto>();
    }
}