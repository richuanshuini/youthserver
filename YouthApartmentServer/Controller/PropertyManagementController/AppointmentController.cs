using Mapster;
using Microsoft.AspNetCore.Mvc;
using YouthApartmentServer.Model.PropertyManagementModel;
using YouthApartmentServer.ModelDto;
using YouthApartmentServer.Services.IAppointmentService;

namespace YouthApartmentServer.Controller.PropertyManagementController;
[ApiController]
[Route("api/[controller]")]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet]
    public async Task<ActionResult<AppointmentDto>> GetAppointment()
    {
        var appointment =await _appointmentService.GetAllAppointmentsAsync();
        return Ok(appointment.Adapt<List<AppointmentDto>>());
    }

    [HttpGet("Nodelete/paged")]
    public async Task<ActionResult<PagedResult<AppointmentDto>>> GetAppointmentPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var paged=await _appointmentService.GetAppointmentsPagedAsync(pageNumber, pageSize);
        var dtoItems=paged.Items.Adapt<List<AppointmentDto>>();
        var dto = new PagedResult<AppointmentDto>
        {
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize,
            Total = paged.Total,
            Items = dtoItems
        };
        return Ok(dto);
    }
    
}