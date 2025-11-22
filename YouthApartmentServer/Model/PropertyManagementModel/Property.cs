using System.Collections;
using FreeSql.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using YouthApartmentServer.Model.UserPermissionModel;
namespace YouthApartmentServer.Model.PropertyManagementModel;

//状态
public enum PropertyStatus
{
    PendingReview=0, //待审核
    Available=1, //可租
    Reserved=2, //已预定
    Rented=3, //已租
    UnderMaintenance=4, //维护中
    Disable=5 //停用
}

//租赁类型
public enum LeaseType
{
    WholeRent=1, //整租
    SharedRent=2, //合租
}

//租赁期限
public enum LeaseTerm
{
    Month=0, // 按月租
    Quarter=1, // 按季租
    HalfYear=2, //半年租
    Year=3 // 按年租
}

public class Property
{
    [Column(IsPrimary = true, IsIdentity = true)] public int PropertyId { get; set; } //房源ID
    public int? RegionId { get; set; } //区域ID
    public int? ApprovedByUser { get; set; } //审核员ID，作为User表的外键，由管理员手动分配
    
    public int? Area { get; set; } //面积
    
    public int? Bedrooms { get; set; } //卧室数量
    
    public int? Bathrooms { get; set; } //浴室数量
    
    public int? MaxTenants { get; set; }  // 最大入住人数
    
    [MaxLength(128)] public string? PropertyName { get; set; } // 房源名称
    
    [MaxLength(256)] public string? Address { get; set; } //地址
    
    [MaxLength(256)] public string? Description { get; set; } //描述
    
    [MaxLength(64)] public string? PropertyCode { get; set; }  // 房源编码，如 GZ-TNH-0502

    [MaxLength(32)] public string? RoomNumber { get; set; }   // 房间号，如 502-A
    
    [Column(Precision = 10, Scale = 2)] public decimal? RentPrice { get; set; } //租赁价格
    
    [Column(Precision = 10, Scale = 2)] public decimal? RentDeposit { get; set; } //租赁押金
    
    [Column(Precision = 10, Scale = 2)] public decimal? PropertyFee { get; set; } //物业费
    
    [Column(Precision = 9, Scale = 6)] public decimal? Latitude { get; set; } // 纬度 (Lat)
    
    [Column(Precision = 9, Scale = 6)] public decimal? Longitude { get; set; } // 经度 (Lng)
    
    public PropertyStatus Status { get; set; } = PropertyStatus.PendingReview; //状态
    
    public LeaseType LeaseType { get; set; } = LeaseType.WholeRent; //租赁类型
    
    public LeaseTerm LeaseTerm { get; set; } = LeaseTerm.Month; //租赁期限
    
    public DateTime? AvailableDate { get; set; } //预约后可入住时间
    
    public DateTime? CreatedAt { get; set; } //创建时间，由ORM决定，初次插入时决定
    
    public DateTime? UpdatedAt { get; set; } //更新时间，由ORM决定，被更新时赋值
    
    public DateTime? ApprovedAt { get; set; } //审核时间
    
    public DateTime? DeletedAt { get; set; } //被删除时间，由IsDeleted决定，被软删除时赋值
    
    public bool IsDeleted { get; set; } = false; //软删除逻辑

    //Property 1:n Appointment，一个个房源可以有多个预约
    [Navigate(nameof(Appointment.PropertyId))]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    //Property n:1 User
    [Navigate(nameof(ApprovedByUser))] public User? User { get; set; }

    //Property 1:n UserProperty
    [Navigate(nameof(UserProperty.PropertyId))]
    public virtual ICollection<UserProperty> Users { get; set; } = new List<UserProperty>();
    
    //Property 1:n PropertyImage
    [Navigate(nameof(PropertyImage.ImageId))]
    public virtual ICollection<PropertyImage> PropertyImages {get;set;} = new List<PropertyImage>();
    
}