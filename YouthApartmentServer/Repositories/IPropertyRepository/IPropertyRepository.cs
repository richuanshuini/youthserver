using FreeSql;
using YouthApartmentServer.Model.PropertyManagementModel;
using YouthApartmentServer.ModelDto;

namespace YouthApartmentServer.Repositories.IPropertyRepository;

public interface IPropertyRepository
{
    ISelect<Property> Query();
    Task<Property?> GetById(int id);
    Task<List<Property>> GetAllAsync();
    Task<Property> InseryAsync(Property property);
    Task<(List<Property> Items, long Total)> GetPagedAsync(int pageNumber, int pageSize);
    Task<bool> UpdateAsync(int id, Property property); //部分更新
    Task<(List<Property> Items, long Total)> SearchAsync(PropertyQueryDto condition, int pageNumber, int pageSize);
}
