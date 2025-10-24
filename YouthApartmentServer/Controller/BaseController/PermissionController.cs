using Mapster;
using Microsoft.AspNetCore.Mvc;
using YouthApartmentServer.ModelDto;
using YouthApartmentServer.Services.IPermissionService;

namespace YouthApartmentServer.Controller.BaseController;
[ApiController]
[Route("api/[controller]")]
public class PermissionController : ControllerBase
{
    private readonly IPermissionService _ipermissionService;
    public PermissionController(IPermissionService ipermissionService)
    {
        _ipermissionService = ipermissionService;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<PermissionDto>>> GetAllPermissons()
    {
        var permissions = await _ipermissionService.GetAllPermissionsAsync();
        return Ok(permissions.Adapt<List<PermissionDto>>());
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<PermissionDto>> GetPermissionById(int id)
    {
        var permission = await _ipermissionService.GetPermissionByIdAsync(id);
        if(permission == null)
            return NotFound(new {error="不存在该权限"});
        var permissionDto=permission.Adapt<PermissionDto>();
        return Ok(permissionDto);
    }
    
    
    
}