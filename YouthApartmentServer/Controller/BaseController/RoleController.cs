using Mapster;
using Microsoft.AspNetCore.Mvc;
using YouthApartmentServer.ModelDto;
using YouthApartmentServer.Services.IRoleServices;

namespace YouthApartmentServer.Controller.BaseController;

[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _iroleService;

    public RoleController(IRoleService iroleService)
    {
        _iroleService = iroleService;
    }
    
    /// <summary>
    /// 查询所有Role
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<RoleDto>>> GetAllRoles()
    {
        var roles = await _iroleService.GetAllRolesAsync();
        return Ok(roles.Adapt<List<RoleDto>>());
    }
        
    /// <summary>
    /// 通过ID查对应的Role
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto?>> GetRoleById(int id)
    {
        var role = await _iroleService.GetRoleByIdAsync(id);
        if (role == null)
        {
            return NotFound(new {error = "该角色不存在"});
        }
        var roleDto = role.Adapt<RoleDto>(); 
        return Ok(roleDto);
    }
    
    
}