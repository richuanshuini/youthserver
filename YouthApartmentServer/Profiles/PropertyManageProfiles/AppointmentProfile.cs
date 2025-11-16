using YouthApartmentServer.Model.PropertyManagementModel;
using YouthApartmentServer.ModelDto;

namespace YouthApartmentServer.Profiles.PropertyManageProfiles;

public class AppointmentProfile
{
    public static void Register(Mapster.TypeAdapterConfig config)
    {
        config.NewConfig<Appointment, AppointmentDto>();
        
    }
    
}