using Mapster;
using Microsoft.AspNetCore.Mvc;
using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.ModelDto;
using YouthApartmentServer.Services.IUserRoleService;

namespace YouthApartmentServer.Controller.BaseController;

[ApiController]
[Route("api/[controller]")]
public class UserRoleController : ControllerBase
{
    private readonly IUserRoleService _iuserRoleService;
    public UserRoleController(IUserRoleService iuserRoleService)
    {
        _iuserRoleService = iuserRoleService;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<UserRoleDto>>> GetAllUserRoles()
    {
        var userRole = await _iuserRoleService.GetAllUserRoleAsync();
        return Ok(userRole.Adapt<List<UserRoleDto>>());
    }

    [HttpPost]
    public async Task<ActionResult<InsertUserRoleDto>> CreateUserRole([FromBody] InsertUserRoleDto insertUserRoleDto)
    {
        var userRole=insertUserRoleDto.Adapt<UserRole>();
        var existUserRole = await _iuserRoleService.CreateUserRoleAsync(userRole);
        if(existUserRole == null)
            return BadRequest(new {error="存在重复的数据或该用户或者角色不存在"});
        return Ok();
    }

    /// <summary>
    /// 批量分配用户-角色
    /// </summary>
    [HttpPost("assign/batch")]
    public async Task<ActionResult> BatchAssignUserRoles([FromBody] BatchAssignUserRolesDto payload)
    {
        var created = await _iuserRoleService.BatchAssignUserRolesAsync(payload.UserIds, payload.RoleIds);
        return Ok(new { created });
    }

    [HttpPost("update")]
    public async Task<ActionResult> UpdateUserRole(int userId,List<int> roleIds)
    {
        var update = await _iuserRoleService.UpdateUserRolesAsync(userId, roleIds);
        if(update)
            return Ok();
        return BadRequest();
    }
    
}