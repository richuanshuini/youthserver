using YouthApartmentServer.Model.ServersModel;
using YouthApartmentServer.ModelDto;
using FreeSql;

namespace YouthApartmentServer.Repositories.IAnnounceMent;

public interface IAnnounceMentRepository
{
    Task<List<AnnounceMent>> GetAllAsync();
    Task<AnnounceMent?> GetByIdAsync(int id);
    Task<AnnounceMent> InsertAsync(AnnounceMent entity);
    Task<bool> UpdateAsync(int id, UpdateAnnouncementDto patchDto);
    Task<bool> SoftDeleteAsync(int id);
    Task<List<AnnounceMent>> GetDeletedAsync();
    Task<AnnounceMent?> GetDeletedByIdAsync(int id);
    Task<bool> RestoreAsync(int id);
    Task<bool> HardDeleteAsync(int id);
    ISelect<AnnounceMent> Query();
}