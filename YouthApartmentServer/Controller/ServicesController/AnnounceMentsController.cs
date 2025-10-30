using Mapster;
using Microsoft.AspNetCore.Mvc;
using YouthApartmentServer.ModelDto;
using YouthApartmentServer.Services.IAnnounceMentService;
using YouthApartmentServer.Model.ServersModel;

namespace YouthApartmentServer.Controller.ServicesController;

[ApiController]
[Route("api/[controller]")]
public class AnnounceMentsController : ControllerBase
{
    private readonly IAnnounceMentService _service;
    private readonly IWebHostEnvironment _env;

    public AnnounceMentsController(IAnnounceMentService service, IWebHostEnvironment env)
    {
        _service = service;
        _env = env;
    }

    /// <summary>
    /// 获取公告列表（不含软删除）
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<AnnouncementDto>>> GetAll()
    {
        var list = await _service.GetAllAsync();
        return Ok(list.Adapt<List<AnnouncementDto>>());
    }

    /// <summary>
    /// 获取回收站公告列表（仅软删除）
    /// </summary>
    [HttpGet("deleted")]
    public async Task<ActionResult<List<AnnouncementDto>>> GetDeleted()
    {
        var list = await _service.GetDeletedAsync();
        return Ok(list.Adapt<List<AnnouncementDto>>());
    }

    /// <summary>
    /// 根据ID获取公告
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AnnouncementDto?>> GetById(int id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound(new { error = "该公告不存在" });
        return Ok(entity.Adapt<AnnouncementDto>());
    }

    /// <summary>
    /// 新增公告
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AnnouncementDto>> Create([FromBody] InsertAnnouncementDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return BadRequest(new { error = "标题不能为空" });
        if (string.IsNullOrWhiteSpace(dto.Content))
            return BadRequest(new { error = "内容不能为空" });

        var created = await _service.CreateAsync(dto);
        var outDto = created.Adapt<AnnouncementDto>();
        return CreatedAtAction(nameof(GetById), new { id = outDto.AnnounceMentId }, outDto);
    }

    /// <summary>
    /// 局部更新公告
    /// </summary>
    [HttpPost("{id}/update")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateAnnouncementDto dto)
    {
        var ok = await _service.UpdateAsync(id, dto);
        if (!ok) return NotFound(new { error = "该公告不存在" });
        return NoContent();
    }

    /// <summary>
    /// 软删除公告
    /// </summary>
    [HttpPost("{id}/delete")]
    public async Task<ActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound(new { error = "该公告不存在或已删除" });
        return NoContent();
    }

    /// <summary>
    /// 恢复公告（从回收站还原）
    /// </summary>
    [HttpPost("{id}/restore")]
    public async Task<ActionResult> Restore(int id)
    {
        var ok = await _service.RestoreAsync(id);
        if (!ok) return NotFound(new { error = "该公告不存在或未被删除" });
        return NoContent();
    }

    /// <summary>
    /// 物理删除公告（永久删除）
    /// </summary>
    [HttpPost("{id}/hard-delete")]
    public async Task<ActionResult> HardDelete(int id)
    {
        var ok = await _service.HardDeleteAsync(id);
        if (!ok) return NotFound(new { error = "该公告不存在" });
        return NoContent();
    }

    /// <summary>
    /// TinyMCE 图片上传，返回可访问的资源URL
    /// </summary>
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10_000_000)] // 约 10MB
    public async Task<ActionResult<TinyMceUploadResult>> Upload([FromForm] TinyMceUploadRequest form)
    {
        var file = form.File;
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "未选择文件" });

        var resourcesRoot = Path.Combine(_env.ContentRootPath, "Resources", "TincyMceImage");
        if (!Directory.Exists(resourcesRoot)) Directory.CreateDirectory(resourcesRoot);

        var ext = Path.GetExtension(file.FileName);
        var safeExt = string.IsNullOrWhiteSpace(ext) ? ".png" : ext;
        var filename = $"{Guid.NewGuid():N}{safeExt}";
        var savePath = Path.Combine(resourcesRoot, filename);

        await using (var stream = System.IO.File.Create(savePath))
        {
            await file.CopyToAsync(stream);
        }

        // 返回绝对 URL，确保前端在不同域/端口下也能正确加载图片
        var absolute = $"{Request.Scheme}://{Request.Host}/resources/TincyMceImage/{filename}";
        return Ok(new TinyMceUploadResult { Location = absolute });
    }
}