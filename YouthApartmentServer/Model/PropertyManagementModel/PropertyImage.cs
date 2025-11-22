using System.ComponentModel.DataAnnotations;
using FreeSql.DataAnnotations;

namespace YouthApartmentServer.Model.PropertyManagementModel;

public class PropertyImage
{
    [Column(IsPrimary = true, IsIdentity = true)]
    public int ImageId { get; set; }
    
    public int PropertyId { get; set; }
    
    [MaxLength(512)]
    public string ImageUrl { get; set; } = string.Empty;

    // 是否封面：标记哪张图在房源列表页展示 (true=封面)
    // 一个房源通常只能有一张封面
    public bool IsCover { get; set; } = false;

    // 排序权重：用于控制轮播图的展示顺序 (数值越小越靠前)
    public int SortOrder { get; set; } = 0;
    
    // PropertyImage n:1 Property ,多对一
    [Navigate(nameof(PropertyId))]
    public Property? Property { get; set; }
}