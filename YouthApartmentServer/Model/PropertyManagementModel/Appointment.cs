using System.ComponentModel.DataAnnotations;
using YouthApartmentServer.Model.UserPermissionModel;
namespace YouthApartmentServer.Model.PropertyManagementModel;

using FreeSql.DataAnnotations;

public enum Appointmentstatus
{
    Pending = 0, //待确认
    Confirmed = 1, //已确认
    Cancelled = 2, //已取消
    Viewed = 3, //已看房
    Missed = 4 //爽约
}

public class Appointment
{
    [Column(IsPrimary = true, IsIdentity = true)] public int AppointmentId { get; set; } //预约ID
    public int? UserId { get; set; } //预约用户
    public int? PropertyId { get; set; } //房源ID
    public int? AssignedStaffId { get; set; } //员工ID，作为User的外键
    [MaxLength(128)] public string? Remarks { get; set; } //备注
    [MaxLength(256)] public string? CancelReason { get; set; }  // 取消原因（手填）
    public DateTime? AppointmentTime { get; set; } //预约开始时间
    public DateTime? AppointmentEndTime { get; set; } //预约结束时间
    public DateTime? AssignmentTime { get; set; } //分配该预约时间，若未分配员工，则可为Null
    public DateTime? CreatedAt { get; set; } //创建时间，由ORM决定，初次插入时决定
    public DateTime? DeletedAt { get; set; } //被删除时间，由IsDeleted决定，被软删除时赋值
    public Appointmentstatus Status { get; set; } = Appointmentstatus.Pending; //预约状态
    public bool IsDeleted { get; set; } = false; //软删除
    
    //Appointment n：1 Property
    [Navigate(nameof(PropertyId))] public Property? Property { get; set; }

    //Appointment n:1 User :User作为员工外键
    [Navigate(nameof(AssignedStaffId))] public User? Staff { get; set; }

    //Appointment n:1 User :User作为预约用户外键，其通过Role赋予的角色区分
    [Navigate(nameof(UserId))] public User? User { get; set; }
}