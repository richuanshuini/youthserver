using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618
using System.Text.Json.Serialization;

namespace YouthApartmentServer.ModelDto;

#region 查询DTO
public class UserDto
{
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? RealName { get; set; }
    public string? IdCard { get; set; }
    public string? Gender { get; set; }
    public string? UserAvatarUrl{get;set;}
    public bool Status { get; set; }
}

public class UserRoleDto
{
    public int UserId { get; set; }
    public string UserName{get;set;}
    public int RoleId { get; set; }
    public string RoleName{get;set;}
}

public class RoleDto
{
    public int RoleId { get; set; }
    public string RoleName { get; set; }
    public string? Description { get; set; }
}

public class RolePermissionDto
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
}

public class PermissionDto
{
    public int PermissionId { get; set; }
    public string? PermissionName { get; set; }
    public string? Description { get; set; }
    public string? Module { get; set; }
}

//用于用户的多条件查询
public class UserQueryParams
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? RealName { get; set; }
    public string? Gender { get; set; }
    public bool? Status { get; set; }
}

// 未分配角色的用户——模糊查询与分页参数
public class UserNoRoleSearchParams
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? RealName { get; set; }
    public string? IdCard { get; set; }
    public string? Gender { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}


#endregion

#region 插入DTO
//这个DTO用于从前端获取插入的书籍，首先隐藏ID（主键自增） 然后就是Status（首次注册都默认需要管理员手动更改状态，才能访问系统）
public class InsertUserDto
{
    [Required(ErrorMessage = "用户名不能为空")]
    [StringLength(30,MinimumLength = 3,ErrorMessage = "用户名长度必须在3~20个字符之间")]
    [RegularExpression(@"^[a-zA-Z0-9]+$",ErrorMessage = "用户名只能包含字母和数字")]
    public string UserName { get; set; }
    
    [Required(ErrorMessage = "密码不能为空")]
    [StringLength(20,MinimumLength = 6,ErrorMessage = "密码的长度必须在6~20个字符之间")]
    [RegularExpression(@"^[a-zA-Z0-9]+$",ErrorMessage = "密码只能包含字母和数字")]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "邮箱不能为空")]
    [EmailAddress(ErrorMessage = "邮箱格式错误")]
    public string Email { get; set; }

    [Required(ErrorMessage = "电话号码不能为空")]
    [RegularExpression(@"^1[3-9]\d{9}$",ErrorMessage = "输入有效的11位中国大陆手机号码")]
    public string Phone { get; set; }
    
    [Required(ErrorMessage = "真实姓名不能为空")]
    public string RealName { get; set; }
    
    [RegularExpression(@"^[1-9]\d{5}(19|20)\d{2}((0[1-9])|(1[0-2]))(([0-2][1-9])|10|20|30|31)\d{3}[0-9Xx]$",ErrorMessage = "请输入有效的身份证号")]
    public string IdCard{get;set;}
    
    [Required(ErrorMessage = "性别不能为空")]
    [RegularExpression(@"^(男|女)$",ErrorMessage = "性别必须为 '男' 或 '女' ")]
    public string Gender { get; set; }
    
    public string? UserAvatarUrl{get;set;}

}

public class InsertUserStatusDto
{
    [Required] 
    public bool? Status { get; set; }
}

public class InsertRoleDto
{
    [Required(ErrorMessage = "角色名称不能为空")]
    public string RoleName { get; set; }
    public string? Description { get; set; }
}

public class InsertUserRoleDto
{
    [Required(ErrorMessage = "用户ID不能为空")]
    public int UserId { get; set; }
    [Required(ErrorMessage = "角色ID不能为空")]
    public int RoleId { get; set; }
    
}

// 批量分配用户-角色请求
public class BatchAssignUserRolesDto
{
    [Required]
    public List<int> UserIds { get; set; } = new();
    [Required]
    public List<int> RoleIds { get; set; } = new();
}

#endregion

#region 修改DTO

public class SetUserStatusDto
{
    public bool Status { get; set; }
}


#endregion

#region 更新DTO

//user更新DTO
public class UpdateUserDto
{
    [Required(ErrorMessage = "用户名不能为空")]
    [StringLength(30,MinimumLength = 3,ErrorMessage = "用户名长度必须在3~20个字符之间")]
    [RegularExpression(@"^[a-zA-Z0-9]+$",ErrorMessage = "用户名只能包含字母和数字")]
    public string? UserName { get; set; }
    
    [Required(ErrorMessage = "密码不能为空")]
    [StringLength(20,MinimumLength = 6,ErrorMessage = "密码的长度必须在6~20个字符之间")]
    [RegularExpression(@"^[a-zA-Z0-9]+$",ErrorMessage = "密码只能包含字母和数字")]
    public string? Password { get; set; }
    
    [Required(ErrorMessage = "邮箱不能为空")]
    [EmailAddress(ErrorMessage = "邮箱格式错误")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "电话号码不能为空")]
    [RegularExpression(@"^1[3-9]\d{9}$",ErrorMessage = "输入有效的11位中国大陆手机号码")]
    public string? Phone { get; set; }
    
    [Required(ErrorMessage = "真实姓名不能为空")]
    public string? RealName { get; set; }
    
    [RegularExpression(@"^[1-9]\d{5}(19|20)\d{2}((0[1-9])|(1[0-2]))(([0-2][1-9])|10|20|30|31)\d{3}[0-9Xx]$",ErrorMessage = "请输入有效的身份证号")]
    public string? IdCard{get;set;}
    
    [Required(ErrorMessage = "性别不能为空")]
    [RegularExpression(@"^(男|女)$",ErrorMessage = "性别必须为 '男' 或 '女' ")]
    public string? Gender { get; set; }
    
    public string? UserAvatarUrl{get;set;}
    public bool Status { get; set; }
}

//制造一个可空DTO，实现部分更新，即检查到前端没有传入内容，就是没更新的部分，就设置为null
public class UpdateRoleDto
{
    [MaxLength(30)] 
    public string? RoleName { get; set; }
    [MaxLength(100)] 
    public string? Description { get; set; }

}


#endregion