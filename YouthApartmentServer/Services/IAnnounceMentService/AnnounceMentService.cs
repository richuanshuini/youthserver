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

    public async Task<ValidationResult<AnnounceMent>> CreateAsync(InsertAnnouncementDto dto)
    {
        var result = new ValidationResult<AnnounceMent>();

        if (dto.PublishTime.HasValue && dto.ExpireTime.HasValue && dto.PublishTime.Value > dto.ExpireTime.Value)
        {
            result.AddError("结束时间不能早于发布时间");
        }

        if (!result.IsValid)
            return result;

        var entity = dto.Adapt<AnnounceMent>();

        if (dto.Status.HasValue)
            entity.Status = (AnnouncementStatus)dto.Status.Value;
        else
            entity.Status = AnnouncementStatus.Draft;

        entity.Type = (AnnouncementType)dto.Type;

        if (entity.Status == AnnouncementStatus.Published && !entity.PublishTime.HasValue)
        {
            entity.PublishTime = DateTime.Now;
        }

        var created = await _repo.InsertAsync(entity);
        result.Data = created;
        return result;
    }

    public async Task<ValidationResult<bool>> UpdateAsync(int id, UpdateAnnouncementDto dto)
    {
        var result = new ValidationResult<bool>();

        if (dto.PublishTime.HasValue && dto.ExpireTime.HasValue && dto.PublishTime.Value > dto.ExpireTime.Value)
        {
            result.AddError("结束时间不能早于发布时间");
            return result;
        }

        var ok = await _repo.UpdateAsync(id, dto);
        if (!ok)
        {
            result.MarkNotFound("该公告不存在");
            return result;
        }

        result.Data = true;
        return result;
    }

    public Task<bool> DeleteAsync(int id) => _repo.SoftDeleteAsync(id);

    public Task<List<AnnounceMent>> GetDeletedAsync() => _repo.GetDeletedAsync();

    public Task<bool> RestoreAsync(int id) => _repo.RestoreAsync(id);

    public Task<bool> HardDeleteAsync(int id) => _repo.HardDeleteAsync(id);
}
