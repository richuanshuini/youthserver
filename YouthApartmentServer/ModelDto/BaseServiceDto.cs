using System.ComponentModel.DataAnnotations;

namespace YouthApartmentServer.ModelDto;

public class AnnouncementDto
{
    public int AnnounceMentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Type { get; set; }
    public int Status { get; set; }
    public DateTime? PublishTime { get; set; }
    public DateTime? ExpireTime { get; set; }
}

// 插入 DTO（写入 TinyMCE HTML 正文）
public class InsertAnnouncementDto
{
    [Required(ErrorMessage = "标题不能为空")]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "内容不能为空")]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "类型不能为空")]
    public int Type { get; set; }

    // 可选：默认草稿
    public int? Status { get; set; }

    public DateTime? PublishTime { get; set; }
    public DateTime? ExpireTime { get; set; }
}

// 局部更新 DTO（仅更新传入的字段）
public class UpdateAnnouncementDto
{
    [MaxLength(255)]
    public string? Title { get; set; }
    public string? Content { get; set; }
    public int? Type { get; set; }
    public int? Status { get; set; }
    public DateTime? PublishTime { get; set; }
    public DateTime? ExpireTime { get; set; }
}