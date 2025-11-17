using Mapster;
using Microsoft.AspNetCore.Mvc;
using YouthApartmentServer.Model.PropertyManagementModel;
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
    public async Task<ActionResult<PropertyDto>> CreatePropety([FromBody]InserPropertyDto inserPropertyDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var property=inserPropertyDto.Adapt<Property>();
        var result=await _propertyService.CreatePropertyAsync(property);
        if(!result.IsValid||result.Data==null)
            return BadRequest(new {error=result.Errors});
        
        var newPropertyDto=result.Data.Adapt<PropertyDto>();
        return CreatedAtAction(nameof(CreatePropety),new {id=result.Data.PropertyId},newPropertyDto);
    }
    
}