using Mapster;
using Microsoft.AspNetCore.Mvc;
using YouthApartmentServer.ModelDto;
using YouthApartmentServer.Services.IRolePermissionService;

namespace YouthApartmentServer.Controller.BaseController;

[ApiController]
[Route("api/[controller]")]
public class RolePermissionController : ControllerBase
{
    private readonly IRolePermissionService _irolePermissionService;
    public RolePermissionController(IRolePermissionService irolePermissionService)
    {
        _irolePermissionService = irolePermissionService;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<RolePermissionDto>>> GetAllRolePermissions()
    {
        var rolePermission = await _irolePermissionService.GetAllRolePermissionsAsync();
        return Ok(rolePermission.Adapt<List<RolePermissionDto>>());
    }
}