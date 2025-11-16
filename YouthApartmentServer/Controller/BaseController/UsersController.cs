using System.Collections.Generic;
using System.Linq;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using YouthApartmentServer.Common.Validation;
using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.ModelDto;
using YouthApartmentServer.Services.IUserService;

namespace YouthApartmentServer.Controller.BaseController;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _iuserService;

    public UsersController(IUserService iuserService)
    {
        _iuserService = iuserService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAllUsers()
    {
        var users = await _iuserService.GetAllUsersAsync();
        return Ok(users.Adapt<List<UserDto>>());
    }

    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<UserDto>>> GetUsersPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var paged = await _iuserService.GetUsersPagedAsync(pageNumber, pageSize);
        var dtoItems = paged.Items.Adapt<List<UserDto>>();
        var dto = new PagedResult<UserDto>
        {
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize,
            Total = paged.Total,
            Items = dtoItems
        };
        return Ok(dto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto?>> GetUserById(int id)
    {
        var user = await _iuserService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { error = "该用户不存在" });
        }
        var userDto = user.Adapt<UserDto>();
        return Ok(userDto);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] InsertUserDto insertUserDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { errors = ModelStateHelper.CollectModelErrors(ModelState) });

        var user = insertUserDto.Adapt<User>();
        var result = await _iuserService.CreateUserAsync(user);
        if (!result.IsValid || result.Data == null)
        {
            return BadRequest(new { errors = result.Errors });
        }

        var newUserDto = result.Data.Adapt<UserDto>();
        return CreatedAtAction(nameof(CreateUser), new { id = result.Data.UserId }, newUserDto);
    }

    [HttpPost("batch")]
    public async Task<ActionResult<UserDto>> CreateListUser([FromBody] List<InsertUserDto> insertUserDtoList)
    {
        if (insertUserDtoList.Count == 0)
            return BadRequest(new { error = "请求体为空或没有任何用户数据" });

        if (!ModelState.IsValid)
            return BadRequest(new { errors = ModelStateHelper.CollectModelErrors(ModelState) });

        var createUsers = new List<User>();
        var errorBag = new List<string>();

        foreach (var userDto in insertUserDtoList)
        {
            var user = userDto.Adapt<User>();
            var insertResult = await _iuserService.CreateUserAsync(user);
            if (!insertResult.IsValid || insertResult.Data == null)
                errorBag.AddRange(insertResult.Errors);
            else
                createUsers.Add(insertResult.Data);
        }

        var createdDtos = createUsers.Adapt<List<UserDto>>();

        return Ok(new
        {
            created = createdDtos,
            errors = errorBag
        });
    }

    [HttpPost("{id}/updateUserStatus")]
    public async Task<IActionResult> SetUserStatus(int id, [FromBody] SetUserStatusDto userStatusDto)
    {
        var result = await _iuserService.UpdateUserStausAsync(id, userStatusDto.Status);
        if (result)
            return Ok();
        return NotFound();
    }

    [HttpPost("{id}/replace")]
    public async Task<ActionResult<UserDto>> RepalceUser(int id, [FromBody] UpdateUserDto updateUserDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { errors = ModelStateHelper.CollectModelErrors(ModelState) });

        var result = await _iuserService.UpdateUserAsync(id, updateUserDto);
        if (result.Status == ValidationStatus.NotFound)
            return NotFound(new { error = result.Errors.FirstOrDefault() ?? "该用户不存在" });
        if (!result.IsValid)
            return BadRequest(new { errors = result.Errors });
        return NoContent();
    }

    [HttpPost("{id}/update")]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { errors = ModelStateHelper.CollectModelErrors(ModelState) });

        var result = await _iuserService.PatchUserAsync(id, updateUserDto);
        if (result.Status == ValidationStatus.NotFound)
            return NotFound(new { error = result.Errors.FirstOrDefault() ?? "该用户不存在" });
        if (!result.IsValid)
            return BadRequest(new { errors = result.Errors });
        return NoContent();
    }

    [HttpPost("search")]
    public async Task<ActionResult<List<UserDto>>> SearcheUserAny([FromBody] UserQueryParams userQueryParams)
    {
        var user = await _iuserService.SearchUserByContain(userQueryParams);
        var userDto = user.Adapt<List<UserDto>>();
        return Ok(userDto);
    }

    [HttpGet("search/NoRoles")]
    public async Task<ActionResult<List<UserDto>>> SearchUserWithNoRole()
    {
        var user = await _iuserService.GetUserWithNoRoleAsync();
        return Ok(user.Adapt<List<UserDto>>());
    }

    [HttpGet("NoRoles/paged")]
    public async Task<ActionResult<PagedResult<UserDto>>> GetUsersNoRolesPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var paged = await _iuserService.GetUsersNoRolesPagedAsync(pageNumber, pageSize);
        var dtoItems = paged.Items.Adapt<List<UserDto>>();
        var dto = new PagedResult<UserDto>
        {
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize,
            Total = paged.Total,
            Items = dtoItems
        };
        return Ok(dto);
    }
}
