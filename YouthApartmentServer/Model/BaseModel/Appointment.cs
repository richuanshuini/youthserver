namespace YouthApartmentServer.Model.BaseModel;

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
    public int AppointmentId{get;set;}
    public int UserId{get;set;}
    public int ProPertyId{get;set;}
    public DateTime AppointMentTime{get;set;}
    public Appointmentstatus Status { get; set; } = Appointmentstatus.Pending;
    public string? Remarks {get;set;}
    public int AssignedStaffId{get;set;}
    public DateTime AssignmentTime{get;set;}
    public bool IsDelete{get;set;}=false;
    public DateTime CreateAt{get;set;}
}