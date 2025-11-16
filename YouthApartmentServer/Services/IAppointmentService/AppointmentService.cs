using YouthApartmentServer.Model.PropertyManagementModel;
using YouthApartmentServer.ModelDto;
using YouthApartmentServer.Repositories.IAppointment;

namespace YouthApartmentServer.Services.IAppointmentService;

public class AppointmentService:IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;

    public AppointmentService(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }


    public async Task<List<Appointment>> GetAllAppointmentsAsync()
    {
        return await _appointmentRepository.GetAllAsync();
    }

    public async Task<PagedResult<Appointment>> GetAppointmentsPagedAsync(int pageNumber, int pageSize)
    {
        var (items,total)= await _appointmentRepository.GetPagedAsync(pageNumber,pageSize);
        //返回分页结果
        return new PagedResult<Appointment>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Total = total,
            Items = items
        };
    }
}