using FreeSql;
using YouthApartmentServer.Model.ServersModel;
using YouthApartmentServer.ModelDto;

namespace YouthApartmentServer.Repositories.IAnnounceMent;

public class AnnounceMentRepository : BaseRepository<AnnounceMent, int>, IAnnounceMentRepository
{
    public AnnounceMentRepository(IFreeSql fsql) : base(fsql) { }

    public ISelect<AnnounceMent> Query() => Select.Where(a => !a.IsDeleted);

    public async Task<List<AnnounceMent>> GetAllAsync()
    {
        return await Query().OrderByDescending(a => a.PublishTime).ToListAsync();
    }

    public async Task<AnnounceMent?> GetByIdAsync(int id)
    {
        return await Query().Where(a => a.AnnounceMentId == id).FirstAsync();
    }

    public async Task<AnnounceMent> InsertAsync(AnnounceMent entity)
    {
        return await base.InsertAsync(entity);
    }

    public async Task<bool> UpdateAsync(int id, UpdateAnnouncementDto patchDto)
    {
        var exists = await GetByIdAsync(id);
        if (exists == null) return false;

        var updater = base.UpdateDiy.Where(a => a.AnnounceMentId == id);
        if (patchDto.Title != null) updater.Set(a => a.Title, patchDto.Title);
        if (patchDto.Content != null) updater.Set(a => a.Content, patchDto.Content);
        if (patchDto.Type.HasValue) updater.Set(a => a.Type, (AnnouncementType)patchDto.Type.Value);
        if (patchDto.Status.HasValue) updater.Set(a => a.Status, (AnnouncementStatus)patchDto.Status.Value);
        if (patchDto.PublishTime.HasValue) updater.Set(a => a.PublishTime, patchDto.PublishTime.Value);
        if (patchDto.ExpireTime.HasValue) updater.Set(a => a.ExpireTime, patchDto.ExpireTime.Value);

        var rows = await updater.ExecuteAffrowsAsync();
        return rows > 0;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var rows = await Orm.Update<AnnounceMent>()
            .Where(a => a.AnnounceMentId == id)
            .Set(a => a.IsDeleted, true)
            .ExecuteAffrowsAsync();
        return rows > 0;
    }
}