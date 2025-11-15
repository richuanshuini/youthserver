using FreeSql.DataAnnotations;
using YouthApartmentServer.Model.UserPermissionModel;
namespace YouthApartmentServer.Model.PropertyManagementModel;
public class UserProperty
{
    [Column(IsPrimary = true, Position = 1)]
    public int UserId { get; set; }

    [Column(IsPrimary = true, Position = 2)]
    public int PropertyId { get; set; }
    
    [Column(Precision = 10, Scale = 2)]
    public decimal? PersonalRentPrice { get; set; }  // 此租客每月租金
    
    [Column(Precision = 10, Scale = 2)]
    public decimal? PersonalDeposit { get; set; }    // 此租客押金
    
    public DateTime? CheckInAt { get; set; }       // 实际入住时间
    public DateTime? CheckOutAt { get; set; }      // 实际退租时间
    public bool IsPrimaryTenant { get; set; }      // 是否为主租人（合租时区分谁是主合同人）
    

    [Navigate(nameof(UserId))]
    public User? User { get; set; }

    [Navigate(nameof(PropertyId))]
    public Property? Property { get; set; }
}
