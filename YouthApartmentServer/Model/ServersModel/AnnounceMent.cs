namespace YouthApartmentServer.Model.ServersModel;

using System;
using System.ComponentModel.DataAnnotations;
using FreeSql.DataAnnotations;

public enum AnnouncementType
{
    System = 1,
    Maintenance = 2,
    Marketing = 3
}

public enum AnnouncementStatus
{
    Draft = 0,
    Published = 1,
    Close = 2
}

public class AnnounceMent
{
    [Column(IsPrimary = true, IsIdentity = true)]
    public int AnnounceMentId { get; set; }

    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    // 为 TinyMCE HTML 正文使用 LONGTEXT
    [Column(DbType = "LONGTEXT")]
    public string Content { get; set; } = string.Empty;

    // 枚举存为 int
    [Column(MapType = typeof(int))]
    public AnnouncementType Type { get; set; } = AnnouncementType.System;
    
    [Column(MapType = typeof(int))]
    public AnnouncementStatus Status { get; set; } = AnnouncementStatus.Draft;
    
    public DateTime? PublishTime { get; set; }
    
    public DateTime? ExpireTime { get; set; }
    
    public bool IsDeleted { get; set; } = false;
}