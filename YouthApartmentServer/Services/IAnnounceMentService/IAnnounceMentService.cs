using YouthApartmentServer.Model.ServersModel;
using YouthApartmentServer.ModelDto;

namespace YouthApartmentServer.Services.IAnnounceMentService;

public interface IAnnounceMentService
{
    Task<List<AnnounceMent>> GetAllAsync();
    Task<AnnounceMent?> GetByIdAsync(int id);
    Task<AnnounceMent> CreateAsync(InsertAnnouncementDto dto);
    Task<bool> UpdateAsync(int id, UpdateAnnouncementDto dto);
    Task<bool> DeleteAsync(int id);
}