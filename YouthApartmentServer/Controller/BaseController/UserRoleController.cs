using Mapster;
using Microsoft.AspNetCore.Mvc;
using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.ModelDto;
using YouthApartmentServer.Services.IUserRoleServices;

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
}