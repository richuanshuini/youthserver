using System.Threading.Tasks.Dataflow;
using YouthApartmentServer.Model.PropertyManagementModel;
using YouthApartmentServer.ModelDto;

namespace YouthApartmentServer.Services.IAppointmentService;

public interface IAppointmentService
{
    Task<List<Appointment>> GetAllAppointmentsAsync();
    Task<PagedResult<Appointment>> GetAppointmentsPagedAsync(int page, int pageSize);
    
}