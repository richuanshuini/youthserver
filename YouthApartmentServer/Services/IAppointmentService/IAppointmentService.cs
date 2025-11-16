using YouthApartmentServer.Model.PropertyManagementModel;
using YouthApartmentServer.ModelDto;

namespace YouthApartmentServer.Services.IAppointmentService;

public interface IAppointmentService
{
    Task<PagedResult<Appointment>> GetAppointmentsPagedAsync(int page, int pageSize);
    
}