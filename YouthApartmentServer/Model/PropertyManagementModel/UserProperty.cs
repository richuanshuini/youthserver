using FreeSql.DataAnnotations;

namespace YouthApartmentServer.Model.PropertyManagementModel;
using YouthApartmentServer.Model.UserPermissionModel;
public class UserProperty
{
    public int UserId{get;set;}
    public int PropertyId{get;set;}
    
    // UserProperty n:1 User
    [Navigate(nameof(UserId))] 
    public User? User { get; set; }
    
    // UserProperty n:1 Property
    [Navigate(nameof(PropertyId))] 
    public Property? Property { get; set; }
}