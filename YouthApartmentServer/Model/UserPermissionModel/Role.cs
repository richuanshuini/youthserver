using System.ComponentModel.DataAnnotations;
using FreeSql.DataAnnotations;

namespace YouthApartmentServer.Model.UserPermissionModel;

public class Role
{
    [Column(IsPrimary = true, IsIdentity = true)]
    public int RoleId { get; set; }

    [MaxLength(30)] public string? RoleName { get; set; }
    [MaxLength(100)] public string? Description { get; set; }

    //导航属性：一对多UserRole的ID，对与当前的来说，指定UserRole的ID作为外键
    [Navigate(nameof(UserRole.RoleId))]
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    
    //导航属性，Role一对多RolePermission，指定RolePermission的RoleId作为外键
    [Navigate(nameof(RolePermission.RoleId))]
    public virtual ICollection<RolePermission> RolePermissions{get;set;} = new List<RolePermission>();
    
}