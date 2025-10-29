using Mapster;
using Microsoft.AspNetCore.Mvc;
using YouthApartmentServer.Model.UserPermissionModel;
using YouthApartmentServer.ModelDto;
using YouthApartmentServer.Services.IUserServices;

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
        /// 分页查询用户
        /// </summary>
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

            //2、执行业务逻辑，如果返回null，说明服务层拦截了重复的用户名
            var newUser = await _iuserService.CreateUserAsync(user);
            if (newUser == null)
            {
                return BadRequest(new { error = "该用户名或者身份证号已经存在，请重新输入" });
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
                error = conflictUsernames.Count > 0 ? "存在冲突的用户名或身份证号" : null,
                conflicts = conflictUsernames
            });
        }
        
        /// <summary>
        /// 更新用户的状态
        /// </summary>
        /// <param name="id">需要更新用户的ID</param>
        /// <param name="userStatusDto">需要更新的DTO，仅包含Status</param>
        /// <returns>204状态码，修改成功</returns>
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
        /// <returns>204状态码，表示更新成功</returns>
        [HttpPost("{id}/replace")]
        public async Task<ActionResult<UserDto>> RepalceUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            // 排除当前用户的唯一性检查
            if (updateUserDto.UserName != null)
            {
                var existed = await _iuserService.ExistUserName(updateUserDto.UserName);
                if (existed != 0 && existed != id)
                {
                    return BadRequest(new { error = "修改后的用户名已经存在，请重新修改" });
                }
            }
            // 身份证唯一性校验（排除自己）
            if (updateUserDto.IdCard != null)
            {
                var ownerId = await _iuserService.ExistIdCard(updateUserDto.IdCard);
                if (ownerId != 0 && ownerId != id)
                {
                    return BadRequest(new { error = "修改后的身份证号已经存在，请重新修改" });
                }
            }

            var result = await _iuserService.UpdateUserAsync(id, updateUserDto);
            if (result == null)
                return NotFound(new { error = "该用户不存在" });
            return NoContent();
        }

        /// <summary>
        /// 部分更新一个用户
        /// </summary>
        /// <param name="id">要更新的用户的ID</param>
        /// <param name="updateUserDto">用户的完整新数据</param>
        /// <returns>204状态码，表示更新成功</returns>
        [HttpPost("{id}/update")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            // 用户名唯一性校验（排除自己）
            if (updateUserDto.UserName != null)
            {
                var existed = await _iuserService.ExistUserName(updateUserDto.UserName);
                if (existed != 0 && existed != id)
                {
                    return BadRequest(new { error = "修改后的用户名已经存在，请重新修改" });
                }
            }
            // 身份证唯一性校验（排除自己）
            if (updateUserDto.IdCard != null)
            {
                var ownerId = await _iuserService.ExistIdCard(updateUserDto.IdCard);
                if (ownerId != 0 && ownerId != id)
                {
                    return BadRequest(new { error = "修改后的身份证号已经存在，请重新修改" });
                }
            }

            var result = await _iuserService.PatchUserAsync(id, updateUserDto);
            if (!result)
                return NotFound(new { error = "该用户不存在" });
            return NoContent();
        }
        
        [HttpPost("search")]
        public async Task<ActionResult<List<UserDto>>> SearcheUserAny([FromBody]UserQueryParams userQueryParams)
        {
            var user= await _iuserService.SearchUserByContain(userQueryParams);
            //转成DTO
            var userDto = user.Adapt<List<UserDto>>();
            
            return Ok(userDto);
        }
        
        
        
    }
}