namespace YouthApartmentServer.Model.PropertyManagementModel;

using FreeSql.DataAnnotations;

public enum Appointmentstatus
{
    Pending=0,    //待确认
    Confirmed=1,    //已确认
    Cancelled=2,    //已取消
    Viewed=3,    //已看房
    Missed=4    //爽约
}

public class Appointment
{
    public int AppointmentId{get;set;} //预约ID
    public int UserId{get;set;} //预约用户
    public int ProPertyId{get;set;} //房源ID
    public int AssignedStaffId{get;set;} //员工ID，作为User的外键
    public string? Remarks {get;set;} //备注
    public DateTime AppointMentTime{get;set;}  //预约开始时间
    public DateTime AppointMentEndTime { get; set; } //预约结束时间
    public DateTime? AssignmentTime{get;set;} //分配该预约时间，若未分配员工，则可为Null
    public DateTime CreateAt{get;set;} //创建时间，由ORM决定，初次插入时决定
    public Appointmentstatus Status { get; set; } = Appointmentstatus.Pending; //预约状态
    public bool IsDelete{get;set;}=false; //软删除
}