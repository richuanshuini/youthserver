using FreeSql;
using YouthApartmentServer.Model.PropertyManagementModel;
namespace YouthApartmentServer.Repositories.IPropertyRepository;

public interface IPropertyRepository
{
    public ISelect<Property> Query();
    public Task<Property?> GetById(int id);
    public Task<List<Property>> GetAllAsync();
    public Task<Property> InseryAsync(Property property);
    public Task<(List<Property> Items,long Total)> GetPagedAsync(int pageNumber, int pageSize);
}