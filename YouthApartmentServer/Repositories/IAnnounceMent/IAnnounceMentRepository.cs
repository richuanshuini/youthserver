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
    ISelect<AnnounceMent> Query();
}