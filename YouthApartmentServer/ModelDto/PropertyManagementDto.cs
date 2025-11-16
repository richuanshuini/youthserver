using System.ComponentModel.DataAnnotations;
using YouthApartmentServer.Model.PropertyManagementModel;
namespace YouthApartmentServer.ModelDto;

public class AppointmentDto
{
    public int AppointmentId { get; set; } //预约ID
    public int? UserId { get; set; } //预约用户
    public int? PropertyId { get; set; } //房源ID
    public int? AssignedStaffId { get; set; } //员工ID，作为User的外键
    public string? Remarks { get; set; } //备注
    public string? CancelReason { get; set; }  // 取消原因（手填）
    public DateTime? AppointmentTime { get; set; } //预约开始时间
    public DateTime? AppointmentEndTime { get; set; } //预约结束时间
    public DateTime? AssignmentTime { get; set; } //分配该预约时间，若未分配员工，则可为Null
    public DateTime? CreatedAt { get; set; } //创建时间，由ORM决定，初次插入时决定
    public DateTime? DeletedAt { get; set; } //被删除时间，由IsDeleted决定，被软删除时赋值
    public Appointmentstatus Status { get; set; }  //预约状态
    public bool IsDeleted { get; set; } = false; //软删除
    public bool IsActive { get; set; } = true;   // 当前是否有效租住关系
}

public class InsertAppointmentDto
{
    [Required(ErrorMessage = "预约用户不能为空")]
    public int UserId { get; set; } //预约用户
    [Required(ErrorMessage = "必须选择预约的房子")]
    public int PropertyId { get; set; } //房源ID
    [Required(ErrorMessage = "处理预约的工作人员必须分配")]
    public int? AssignedStaffId { get; set; } //员工ID，作为User的外键
    [StringLength(128,MinimumLength = 0,ErrorMessage = "备注不能超过128个字符")]
    public string? Remarks { get; set; } //备注
    [Required(ErrorMessage = "取消原因必填")]
    public string? CancelReason { get; set; }  // 取消原因（手填）
    public DateTime? AppointmentTime { get; set; } //预约开始时间
    public DateTime? AppointmentEndTime { get; set; } //预约结束时间
    public DateTime? AssignmentTime { get; set; } //分配该预约时间，若未分配员工，则可为Null
    public DateTime? CreatedAt { get; set; } //创建时间，由ORM决定，初次插入时决定
    public DateTime? DeletedAt { get; set; } //被删除时间，由IsDeleted决定，被软删除时赋值
    public Appointmentstatus Status { get; set; }  //预约状态
    public bool IsDeleted { get; set; } = false; //软删除
    public bool IsActive { get; set; } = true;   // 当前是否有效租住关系
}

