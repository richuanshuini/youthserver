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
    public int? RegionId { get; set; } //区域ID
    [Required(ErrorMessage = "审核员必须分配")]
    public int ApprovedByUser { get; set; } //审核员ID，作为User表的外键，由管理员手动分配
    
    [Required(ErrorMessage = "房源面积必须填写")]
    [Range(1, int.MaxValue, ErrorMessage = "房源面积必须大于 1")]
    public int Area { get; set; } //面积
    
    [Required(ErrorMessage = "卧室数量必须填写")]
    [Range(1,Int32.MaxValue, ErrorMessage = "卧室数量必须大于1")]
    public int Bedrooms { get; set; } //卧室数量
    
    [Required(ErrorMessage = "浴室数量必须填写")]
    [Range(1, Int32.MaxValue, ErrorMessage = "浴室数量必须大于1")]
    public int Bathrooms { get; set; } //浴室数量
    
    [Required(ErrorMessage = "最大入住人数必须填写")]
    [Range(1, Int32.MaxValue, ErrorMessage = "最多入住人数必须大于1")]
    public int MaxTenants { get; set; }  // 建议最多入住人数
    
    [Required(ErrorMessage = "房源名称必须填写")]
    [StringLength(128,MinimumLength = 2,ErrorMessage = "房源名称必须在2~128字符之间")]
    public string PropertyName { get; set; } // 房源名称
    
    [Required(ErrorMessage = "房源地址必须填写")]
    [StringLength(256,MinimumLength = 2,ErrorMessage = "地址必须在2~256个字符之间")]
    public string Address { get; set; } //地址
    
    [Required(ErrorMessage = "房源描述必须填写")]
    [StringLength(256,MinimumLength = 2,ErrorMessage = "房源描述必须在2~256个字符之间")]
    public string Description { get; set; } //描述
    
    [Required(ErrorMessage = "房源编号必须填写")]
    [StringLength(64,MinimumLength = 2,ErrorMessage = "房源编号必须在2~64个字符之间")]
    public string PropertyCode { get; set; }  // 房源编码，如 GZ-TNH-0502
    
    [Required(ErrorMessage = "房间号必须填写")]
    [StringLength(32,MinimumLength = 2,ErrorMessage = "房间号必须在2~32个字符之间")]
    public string RoomNumber { get; set; }   // 房间号，如 502-A
    
    [Required(ErrorMessage = "租赁价格必须填写")]
    [Range(typeof(decimal), "0.00", "9999999999.99", ErrorMessage = "租赁价格必须大于等于 0")]
    public decimal RentPrice { get; set; } //租赁价格
    
    [Required(ErrorMessage = "租赁押金必须填写")]
    [Range(typeof(decimal), "0.00", "9999999999.99", ErrorMessage = "租赁押金价格必须大于等于 0")]
    public decimal RentDeposit { get; set; } //租赁押金
    
    [Required(ErrorMessage = "物业费必须填写")]
    [Range(typeof(decimal), "0.00", "9999999999.99", ErrorMessage = "物业费必须大于等于 0")]
    public decimal PropertyFee { get; set; } //物业费
    
    [Required(ErrorMessage = "纬度必须填写")]
    [Range(-90, 90, ErrorMessage = "纬度必须在 -90 到 90 之间")]
    public decimal Latitude { get; set; } // 纬度 (Lat)
    
    [Required(ErrorMessage = "经度必须填写")]
    [Range(-180, 180, ErrorMessage = "经度必须在 -180 到 180 之间")]
    public decimal Longitude { get; set; } // 经度 (Lng)
    
    [Required(ErrorMessage = "租赁类型必须填写")]
    public LeaseType LeaseType { get; set; } //租赁类型
    
    [Required(ErrorMessage = "租赁期限必须填写")]
    public LeaseTerm LeaseTerm { get; set; } //租赁期限
}

public class UpdatePropertyDto
{
    public int? RegionId { get; set; } //区域ID
    
    [Required(ErrorMessage = "审核员必须分配")]
    public int? ApprovedByUser { get; set; } //审核员ID，作为User表的外键，由管理员手动分配
    
    [Required(ErrorMessage = "房源面积必须填写")]
    [Range(1, int.MaxValue, ErrorMessage = "房源面积必须大于 1")]
    public int? Area { get; set; } //面积
    
    [Required(ErrorMessage = "卧室数量必须填写")]
    [Range(1,Int32.MaxValue, ErrorMessage = "卧室数量必须大于1")]
    public int? Bedrooms { get; set; } //卧室数量
    
    [Required(ErrorMessage = "浴室数量必须填写")]
    [Range(1, Int32.MaxValue, ErrorMessage = "浴室数量必须大于1")]
    public int? Bathrooms { get; set; } //浴室数量
    
    [Required(ErrorMessage = "最大入住人数必须填写")]
    [Range(1, Int32.MaxValue, ErrorMessage = "最多入住人数必须大于1")]
    public int? MaxTenants { get; set; }  // 建议最多入住人数
    
    [Required(ErrorMessage = "房源名称必须填写")]
    [StringLength(128,MinimumLength = 2,ErrorMessage = "房源名称必须在2~128字符之间")]
    public string? PropertyName { get; set; } // 房源名称
    
    [Required(ErrorMessage = "房源地址必须填写")]
    [StringLength(256,MinimumLength = 2,ErrorMessage = "地址必须在2~256个字符之间")]
    public string? Address { get; set; } //地址
    
    [Required(ErrorMessage = "房源描述必须填写")]
    [StringLength(256,MinimumLength = 2,ErrorMessage = "房源描述必须在2~256个字符之间")]
    public string? Description { get; set; } //描述
    
    [Required(ErrorMessage = "房源编号必须填写")]
    [StringLength(64,MinimumLength = 2,ErrorMessage = "房源编号必须在2~64个字符之间")]
    public string? PropertyCode { get; set; }  // 房源编码，如 GZ-TNH-0502
    
    [Required(ErrorMessage = "房间号必须填写")]
    [StringLength(32,MinimumLength = 2,ErrorMessage = "房间号必须在2~32个字符之间")]
    public string? RoomNumber { get; set; }   // 房间号，如 502-A
    
    [Required(ErrorMessage = "租赁价格必须填写")]
    [Range(typeof(decimal), "0.00", "9999999999.99", ErrorMessage = "租赁价格必须大于等于 0")]
    public decimal? RentPrice { get; set; } //租赁价格
    
    [Required(ErrorMessage = "租赁押金必须填写")]
    [Range(typeof(decimal), "0.00", "9999999999.99", ErrorMessage = "租赁押金价格必须大于等于 0")]
    public decimal? RentDeposit { get; set; } //租赁押金
    
    [Required(ErrorMessage = "物业费必须填写")]
    [Range(typeof(decimal), "0.00", "9999999999.99", ErrorMessage = "物业费必须大于等于 0")]
    public decimal? PropertyFee { get; set; } //物业费
    
    [Required(ErrorMessage = "纬度必须填写")]
    [Range(-90, 90, ErrorMessage = "纬度必须在 -90 到 90 之间")]
    public decimal? Latitude { get; set; } // 纬度 (Lat)
    
    [Required(ErrorMessage = "经度必须填写")]
    [Range(-180, 180, ErrorMessage = "经度必须在 -180 到 180 之间")]
    public decimal? Longitude { get; set; } // 经度 (Lng)
    
    [Required(ErrorMessage = "租赁类型必须填写")]
    public LeaseType? LeaseType { get; set; } //租赁类型
    
    [Required(ErrorMessage = "租赁期限必须填写")]
    public LeaseTerm? LeaseTerm { get; set; } //租赁期限
    
    public DateTime? AvailableDate { get; set; } //预约后可入住时间
    
}

public class PropertyQueryDto
{
    // 1. 关键字搜索：房源名 / 编码 / 地址 / 房间号
    public string? Keyword { get; set; }

    // 2. 区域、状态、类型
    public int? RegionId { get; set; }
    public PropertyStatus? Status { get; set; }
    public LeaseType? LeaseType { get; set; }
    public LeaseTerm? LeaseTerm { get; set; }

    // 3. 价格、面积、户型等区间
    public decimal? MinRentPrice { get; set; }
    public decimal? MaxRentPrice { get; set; }

    public int? MinArea { get; set; }
    public int? MaxArea { get; set; }

    public int? MinBedrooms { get; set; }
    public int? MaxBedrooms { get; set; } 

    public int? MinBathrooms { get; set; }
    public int? MaxBathrooms { get; set; }

    // 4. 时间维度筛选
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }

    public DateTime? ApprovedFrom { get; set; }
    public DateTime? ApprovedTo { get; set; }

    public DateTime? AvailableFrom { get; set; }
    public DateTime? AvailableTo { get; set; }

    // 5. 审核相关
    public int? ApprovedByUser { get; set; }

    // 6. 软删除相关
    public bool? IsDeleted { get; set; }
}
