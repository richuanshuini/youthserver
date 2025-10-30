using Mapster;
using YouthApartmentServer.Model.ServersModel;
using YouthApartmentServer.ModelDto;
using YouthApartmentServer.Repositories.IAnnounceMent;

namespace YouthApartmentServer.Services.IAnnounceMentService;

public class AnnounceMentService : IAnnounceMentService
{
    private readonly IAnnounceMentRepository _repo;

    public AnnounceMentService(IAnnounceMentRepository repo)
    {
        _repo = repo;
    }

    public Task<List<AnnounceMent>> GetAllAsync() => _repo.GetAllAsync();

    public Task<AnnounceMent?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

    public async Task<AnnounceMent> CreateAsync(InsertAnnouncementDto dto)
    {
        var entity = dto.Adapt<AnnounceMent>();
        // 默认状态：未提供则草稿
        if (dto.Status.HasValue)
            entity.Status = (AnnouncementStatus)dto.Status.Value;
        else
            entity.Status = AnnouncementStatus.Draft;

        entity.Type = (AnnouncementType)dto.Type;

        // 业务优化：发布状态且未指定发布时刻，则使用当前时间
        if (entity.Status == AnnouncementStatus.Published && !entity.PublishTime.HasValue)
        {
            entity.PublishTime = DateTime.Now;
        }

        return await _repo.InsertAsync(entity);
    }

    public Task<bool> UpdateAsync(int id, UpdateAnnouncementDto dto)
    {
        // 当 Status 更新为 Published 且未提供 PublishTime，则由仓储保持原值；
        // 如需设置当前时间，可在控制器层补足逻辑，这里保持轻量。
        return _repo.UpdateAsync(id, dto);
    }

    public Task<bool> DeleteAsync(int id) => _repo.SoftDeleteAsync(id);
}