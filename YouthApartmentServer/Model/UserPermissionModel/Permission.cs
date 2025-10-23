using System.ComponentModel.DataAnnotations;
using FreeSql.DataAnnotations;

namespace YouthApartmentServer.Model.UserPermissionModel;

public class Permission
{
    [Column(IsIdentity = true, IsPrimary = true)]
    public int PermissionId { get; set; }
    [MaxLength(50)]
    public string? PermissionName { get; set; }
    [MaxLength(100)]
    public string? Description { get; set; }
    [MaxLength(64)]
    public string? Module { get; set; }

    //导航属性：Permission多对一RolePermission，指定PermissionId为外键
    [Navigate(nameof(RolePermission.PermissionId))]
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}