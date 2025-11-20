using System.Diagnostics;
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
    
    /// <summary>
    /// 新增房源
    /// </summary>
    /// <param name="inserPropertyDto"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 多条件并查询
    /// </summary>
    [HttpPost("search")]
    public async Task<ActionResult<PagedResult<PropertyDto>>> Search([FromBody] PropertyQueryDto queryDto,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var pagedResult = await _propertyService.SearchPropertiesAsync(queryDto, pageNumber, pageSize);
        var dtoItems = pagedResult.Items.Adapt<List<PropertyDto>>();

        var response = new PagedResult<PropertyDto>
        {
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            Total = pagedResult.Total,
            Items = dtoItems
        };

        return Ok(response);
    }

    [HttpGet("Nodelete/paged")]
    public async Task<ActionResult<PagedResult<PropertyDto>>> GetPropertyPaged([FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var paged = await _propertyService.GetPropertyPagedAsync(pageNumber, pageSize);
        var dtoItems = paged.Items.Adapt<List<PropertyDto>>();
        var dto = new PagedResult<PropertyDto>
        {
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize,
            Total = paged.Total,
            Items = dtoItems,
        };
        return Ok(dto);
    }
    
    /// <summary>
    /// 部分更新
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updatePropertyDto"></param>
    /// <returns></returns>
    [HttpPost("{id}/update")]
    public async Task<ActionResult> UpdateProperty([FromRoute] int id,
        [FromBody] UpdatePropertyDto updatePropertyDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var dto=updatePropertyDto.Adapt<Property>();
        var result=await _propertyService.UpdatePropertyAsync(id,dto);
        if (result.Status == ValidationStatus.NotFound)
            return NotFound(new { errors = result.Errors });
        if (!result.IsValid)
            return BadRequest(new { errors = result.Errors });
        Console.WriteLine("测试代码");
        return NoContent();
    }
    
}
