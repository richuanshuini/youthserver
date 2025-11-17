using Mapster;
using Microsoft.AspNetCore.Mvc;
using YouthApartmentServer.ModelDto;
using YouthApartmentServer.Services.IPropertyService;

namespace YouthApartmentServer.Controller.PropertyManagementController;

[ApiController]
[Route("api/[controller]")]
public class PropertyController : ControllerBase
{
    private readonly IPropertyService _propertyService;

    public PropertyController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    [HttpGet]
    public async Task<ActionResult<List<PropertyDto>>> GetAllProperty()
    {
        var property =  await _propertyService.GetAllPropertyAsync();
        return Ok(property.Adapt<List<PropertyDto>>());
    }

    [HttpPost]
    public async Task<ActionResult<PropertyDto>> CreatePropety(InserPropertyDto inserPropertyDto)
    {
        
    }
    
}