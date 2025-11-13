using System.ComponentModel.DataAnnotations.Schema;

namespace YouthApartmentServer.Model.PropertyManagementModel;

using FreeSql.DataAnnotations;

//状态
public enum PropertyStatus
{
    PendingReview, //待审核
    Available, //可租
    Reserved, //已预定
    Rented, //已租
    UnderMaintenance, //维护中
    Disable //停用
}

//租赁类型
public enum LeaseType
{
    WholeRent, //整租
    SharedRent, //合租
    ShortTerm, //短租
    LongTrem, //长租
}

//租赁期限
public enum LeaseTerm
{
    Month, // 按月租
    Quarter, // 按季租
    HalfYear, //半年租
    Year // 按年租
}

public class Property
{
    public int PropertyId { get; set; } //房源ID
    public int RegionId { get; set; } //区域ID
    public int ApprovedById { get; set; } //审核员ID，作为User表的外键
    public int Area { get; set; } //面积
    public int Bedrooms { get; set; } //卧室数量
    public int Bathrooms { get; set; } //浴室数量
    public string? PropertyName { get; set; } // 房源名称
    public string? Address { get; set; } //地址
    public string? Description { get; set; } //描述
    public decimal RentPrice { get; set; } //租赁价格
    public decimal RentDeposit { get; set; } //租赁押金
    public decimal PropertyFee { get; set; } //物业费
    public decimal Latitude { get; set; } //经度
    public decimal Lngitude { get; set; } //纬度
    public PropertyStatus Status { get; set; } = PropertyStatus.PendingReview; //状态
    public LeaseType LeaseType { get; set; } = LeaseType.WholeRent; //租赁类型
    public LeaseTerm LeaseTerm { get; set; } = LeaseTerm.Month; //租赁期限
    public DateTime AvailableDate { get; set; } //预约后可入住时间
    public DateTime CraeteAt { get; set; } //创建时间，由ORM决定，初次插入时决定
    public DateTime UpdateAt { get; set; } //更新时间，由ORM决定，被更新时赋值
    public DateTime ApprovedBy { get; set; } //审核时间
    public DateTime DeleteAt { get; set; } //被删除时间，由IsDeleted决定，被软删除时赋值
    public bool IsDeleted { get; set; } = false; //软删除逻辑
    
    //Property 1:n Appointment，房源和预约是多对一关系
    [Navigate(nameof(Appointment.ProPertyId))]
    public List<Appointment>? Appointments {get;set;}
    
}