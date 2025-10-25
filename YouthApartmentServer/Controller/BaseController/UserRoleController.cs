using Mapster;
using Microsoft.AspNetCore.Mvc;
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
}