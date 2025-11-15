using System.ComponentModel.DataAnnotations;
using FreeSql.DataAnnotations;
using YouthApartmentServer.Model.PropertyManagementModel;

namespace YouthApartmentServer.Model.UserPermissionModel;

[Index("uk_username", "UserName", true)]
[Index("uk_IdCard", "IdCard", true)]
public class User
{
    [Column(IsPrimary = true,IsIdentity = true)]
    public int UserId { get; set; }
    [MaxLength(30)]
    public string? UserName { get; set; }
    [MaxLength(20)]
    public string? Password { get; set; }
    [MaxLength(50)]
    public string? Email { get; set; }
    [MaxLength(30)]
    public string? Phone { get; set; }
    [MaxLength(30)]
    public string? RealName { get; set; }
    [MaxLength(18)] 
    public string? IdCard { get; set; }
    [MaxLength(2)]
    public string? Gender { get; set; }
    [MaxLength(500)]
    public string? UserAvatarUrl{get;set;}
    public bool? Status { get; set; }=false;

    //导航属性：UserId一对多UserRole的UserId
    //这里的属性名称，不能跟类型一样，就是UserRoles不能变成UserRole，根据就近原则，nameof会查找最近的UserRole，当前有两个，当前类和类外面的，他会优先查找当前类的，也就是List<UserRole>的方法
    //也就是List的方法，有add,remove等方法，但是就是没有UserId这个方法，也就是说他就会报错了，
    //如果不重名，就会去命名空间里面找，然后找到UserRole.cs，里面有UserId
    [Navigate(nameof(UserRole.UserId))]
    public virtual ICollection<UserRole> UserRoles{get;set;} = new List<UserRole>();
    
    //User 1:n UserProperty 从User中查找租户
    [Navigate(nameof(UserProperty.UserId))]
    public virtual ICollection<UserProperty> UserProperties { get; set;} =new List<UserProperty>();
    
    //User 1:n Property 从User中查找审核员
    [Navigate(nameof(Property.ApprovedByUser))]
    public virtual ICollection<Property> PropertyApprove{get;set;}=new List<Property>();
    
    //User 1:n Appointment 从User查找员工
    [Navigate(nameof(Appointment.AssignedStaffId))]
    public virtual ICollection<Appointment>? AppointmentStaffs {get;set;} =new List<Appointment>();
    
    //User 1:n Appointment  从User查找预约用户
    [Navigate(nameof(Appointment.UserId))]
    public virtual ICollection<Appointment>? AppointmentUsers {get;set;}=new List<Appointment>();
    
    
    
}