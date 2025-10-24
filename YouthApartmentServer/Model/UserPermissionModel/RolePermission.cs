using FreeSql.DataAnnotations;
namespace YouthApartmentServer.Model.UserPermissionModel;

public class RolePermission
{
    [Column(IsPrimary = true, Position = 1)]
    public int RoleId { get; set; }
    [Column(IsPrimary = true, Position = 2)]
    public int PermissionId { get; set; }
    
    //导航属性：多对一，查找当前类的RoleId，与Role.RoleId（主键）关联
    [Navigate(nameof(RoleId))]
    public Role? Role {get;set;}
    
    //导航属性：多对一，查找当前类的PermissionId，与Permission.PermissionId（主键）关联
    [Navigate(nameof(PermissionId))]
    public Permission? Permission {get;set;}
    
}