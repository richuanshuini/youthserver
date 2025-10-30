using Mapster;
using Microsoft.AspNetCore.Mvc;
using YouthApartmentServer.Model.UserPermissionModel;
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
    public async Task<ActionResult<RoleDto?>> GetRoleById(int id)
    {
        var role = await _iroleService.GetRoleByIdAsync(id);
        if (role == null)
        {
            return NotFound(new { error = "该角色不存在" });
        }
        var roleDto = role.Adapt<RoleDto>();
        return Ok(roleDto);
    }

    [HttpPost]
    public async Task<ActionResult<RoleDto>> CreateRole([FromBody] InsertRoleDto insertRoleDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var role = insertRoleDto.Adapt<Role>();
        var newRole = await _iroleService.CreateRoleAsync(role);
        var newRoleDto = newRole.Adapt<RoleDto>();
        return CreatedAtAction(nameof(GetRoleById), new { id = newRoleDto.RoleId }, newRoleDto);
    }

    [HttpPost("{id}/update")]
    public async Task<ActionResult<RoleDto>> UpdateRole(int id, [FromBody] UpdateRoleDto updateRoleDto)
    {
        var result = await _iroleService.UpdateRoleAsync(id, updateRoleDto);
        if (!result)
            return BadRequest(new { error = "该角色不存在" });
        return NoContent();
    }
    
    
    
}