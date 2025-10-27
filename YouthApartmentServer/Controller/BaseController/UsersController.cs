using Mapster;
using Microsoft.AspNetCore.Mvc;
using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.ModelDto;
using YouthApartmentServer.Services.IUserServices;
using ZstdSharp.Unsafe;

namespace YouthApartmentServer.Controller.BaseController
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _iuserService;

        public UsersController(IUserService iuserService)
        {
            _iuserService = iuserService;
        }
        
        /// <summary>
        /// 查询所有User
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            var users = await _iuserService.GetAllUsersAsync();
            return Ok(users.Adapt<List<UserDto>>());
        }
        
        /// <summary>
        /// 通过ID查对应的User
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto?>> GetUserById(int id)
        {
            var user = await _iuserService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new {error = "该用户不存在"});
            }
            var userDto = user.Adapt<UserDto>(); // 替换为 Mapster 映射
            return Ok(userDto);
        }
        
        /// <summary>
        /// 创建单个用户
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] InsertUserDto insertUserDto)
        {
            //1、使用 Mapster 将 DTO 转为 User
            var user = insertUserDto.Adapt<User>(); 

            //2、执行业务逻辑
            var newUser = await _iuserService.CreateUserAsync(user);

            if (newUser == null)
            {
                return BadRequest(new { error = "该用户名已经存在，请重新输入" });
            }

            //3、返回 DTO
            var newUserDto = newUser.Adapt<UserDto>(); // 替换为 Mapster 映射
            return CreatedAtAction(nameof(CreateUser), new { id = newUser.UserId }, newUserDto);
        }
        
        /// <summary>
        /// 批量插入User数据
        /// </summary>
        /// <param name="insertUserDtoList">前端传入的批量数据</param>
        /// <returns>插入后的汇总结果</returns>
        [HttpPost("batch")]
        public async Task<ActionResult<UserDto>> CreateListUser([FromBody] List<InsertUserDto> insertUserDtoList)
        {
            if (insertUserDtoList.Count == 0)
                return BadRequest(new { error = "请求体为空或没有任何用户数据" });

            var createUsers = new List<User>();
            var conflictUsernames = new List<string>();

            foreach (var userDto in insertUserDtoList)
            {
                // 将传入的数据映射为 User（Mapster）
                var user = userDto.Adapt<User>(); //Mapster 映射

                var insertUser = await _iuserService.CreateUserAsync(user);
                if (insertUser == null)
                    conflictUsernames.Add(userDto.UserName);
                else
                    createUsers.Add(insertUser);
            }

            var createdDtos = createUsers.Adapt<List<UserDto>>(); // 汇总成功项转 DTO

            return Ok(new
            {
                created = createdDtos,
                error = conflictUsernames.Count > 0 ? "存在冲突的用户名" : null,
                conflicts = conflictUsernames
            });
        }
        
        
        [HttpPost("{id}/updateUserStatus")]
        public async Task<IActionResult> SetUserStatus(int id, [FromBody] SetUserStatusDto userStatusDto)
        {
            var result = await _iuserService.UpdateUserStausAsync(id, userStatusDto.Status);
            if(result)
                return Ok();
            return NotFound();
        }
        
        /// <summary>
        /// 全量更新一个用户
        /// </summary>
        /// <param name="id">要更新的用户的ID</param>
        /// <param name="updateUserDto">用户的完整新数据</param>
        /// <returns>更新后的用户信息</returns>
        [HttpPost("{id}/replace")]
        public async Task<ActionResult<UserDto>> RepalceUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            var result=await _iuserService.UpdateUserAsync(id, updateUserDto);
            if (result == null)
                return NotFound(new { error = "该用户不存在" });
            return Ok(result.Adapt<UserDto>());
        }
        
        /// <summary>
        /// 部分更新用户
        /// </summary>
        /// <param name="id">要更新的用户的ID</param>
        /// <param name="updateUserDto">用户的部分新数据</param>
        /// <returns>204状态码，表示更新成功</returns>
        [HttpPost("{id}/update")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            var result=await _iuserService.PatchUserAsync(id, updateUserDto);
            if (!result)
                return NotFound(new { error = "该用户不存在" });
            return NoContent();
        }
        
        
    }
}