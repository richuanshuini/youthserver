using YouthApartmentServer.Model.PropertyManagementModel;
using YouthApartmentServer.ModelDto;

namespace YouthApartmentServer.Services.IPropertyService;

public interface IPropertyService
{
    Task<List<Property>> GetAllPropertyAsync();
    Task<ValidationResult<Property>> CreatePropertyAsync(Property property);
    Task<PagedResult<Property>> GetPropertyPagedAsync(int pageNumber, int pageSize);
}