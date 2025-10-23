using FreeSql.DataAnnotations;

namespace YouthApartmentServer.Model.UserPermissionModel;

public class UserRole
{
    [Column(IsPrimary = true, Position = 1)]
    public int UserId { get; set; }

    [Column(IsPrimary = true, Position = 2)]
    public int RoleId { get; set; }

    //导航属性：多对一，对于UserRole到User来说，是多对一关系，
    [Navigate(nameof(UserId))] 
    public User? User { get; set; }
    
    //导航属性：多对一，对于UserRole到Role来说，是多对一关系，
    [Navigate(nameof(RoleId))] 
    public Role? Role { get; set; }
    
    
}