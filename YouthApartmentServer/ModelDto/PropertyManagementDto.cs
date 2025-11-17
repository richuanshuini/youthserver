using System.ComponentModel.DataAnnotations;
using YouthApartmentServer.Model.PropertyManagementModel;
#pragma warning disable CS8618
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
}

//插入DTO，要综合考虑相关字段是否默认插入
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
    
    public DateTime? AppointmentTime { get; set; } //预约开始时间
    
    public DateTime? AppointmentEndTime { get; set; } //预约结束时间
    
}

public class PropertyDto
{
    public int? RegionId { get; set; } //区域ID
    public int? ApprovedByUser { get; set; } //审核员ID，作为User表的外键，由管理员手动分配
    public int? Area { get; set; } //面积
    public int? Bedrooms { get; set; } //卧室数量
    public int? Bathrooms { get; set; } //浴室数量
    public int? MaxTenants { get; set; }  // 建议最多入住人数
    public string? PropertyName { get; set; } // 房源名称
    public string? Address { get; set; } //地址
    public string? Description { get; set; } //描述
    public string? PropertyCode { get; set; }  // 房源编码，如 GZ-TNH-0502
    public string? RoomNumber { get; set; }   // 房间号，如 502-A
    public decimal? RentPrice { get; set; } //租赁价格
    public decimal? RentDeposit { get; set; } //租赁押金
    public decimal? PropertyFee { get; set; } //物业费
    public decimal? Latitude { get; set; } // 纬度 (Lat)
    public decimal? Longitude { get; set; } // 经度 (Lng)
    public PropertyStatus Status { get; set; } //状态
    public LeaseType LeaseType { get; set; } //租赁类型
    public LeaseTerm LeaseTerm { get; set; } //租赁期限
    public DateTime? AvailableDate { get; set; } //预约后可入住时间
    public DateTime? CreatedAt { get; set; } //创建时间，由ORM决定，初次插入时决定
    public DateTime? UpdatedAt { get; set; } //更新时间，由ORM决定，被更新时赋值
    public DateTime? ApprovedAt { get; set; } //审核时间
    public DateTime? DeletedAt { get; set; } //被删除时间，由IsDeleted决定，被软删除时赋值
    public bool IsDeleted { get; set; } //软删除逻辑
}

public class InserPropertyDto
{
    public class PropertyDto
    {
        public int? RegionId { get; set; } //区域ID
        [Required(ErrorMessage = "审核员必须分配")]
        public int ApprovedByUser { get; set; } //审核员ID，作为User表的外键，由管理员手动分配
        [Required(ErrorMessage = "房源面积必须填写")]
        public int Area { get; set; } //面积
        [Required(ErrorMessage = "卧室数量必须填写")]
        public int Bedrooms { get; set; } //卧室数量
        [Required(ErrorMessage = "浴室数量必须填写")]
        public int Bathrooms { get; set; } //浴室数量
        [Required(ErrorMessage = "最大入住人数必须填写")]
        public int MaxTenants { get; set; }  // 建议最多入住人数
        [Required(ErrorMessage = "房源名称必须填写")]
        public string PropertyName { get; set; } // 房源名称
        [Required(ErrorMessage = "房源地址必须填写")]
        public string Address { get; set; } //地址
        [Required(ErrorMessage = "房源描述必须填写")]
        public string Description { get; set; } //描述
        [Required(ErrorMessage = "房源编号必须填写")]
        public string PropertyCode { get; set; }  // 房源编码，如 GZ-TNH-0502
        [Required(ErrorMessage = "房间号必须填写")]
        public string RoomNumber { get; set; }   // 房间号，如 502-A
        [Required(ErrorMessage = "租赁价格必须填写")]
        public decimal RentPrice { get; set; } //租赁价格
        [Required(ErrorMessage = "租赁押金必须填写")]
        public decimal RentDeposit { get; set; } //租赁押金
        [Required(ErrorMessage = "物业费必须填写")]
        public decimal PropertyFee { get; set; } //物业费
        [Required(ErrorMessage = "纬度必须填写")]
        public decimal Latitude { get; set; } // 纬度 (Lat)
        [Required(ErrorMessage = "经度必须填写")]
        public decimal Longitude { get; set; } // 经度 (Lng)
        [Required(ErrorMessage = "租赁类型必须填写")]
        public LeaseType LeaseType { get; set; } //租赁类型
        [Required(ErrorMessage = "租赁期限必须填写")]
        public LeaseTerm LeaseTerm { get; set; } //租赁期限
    }
}