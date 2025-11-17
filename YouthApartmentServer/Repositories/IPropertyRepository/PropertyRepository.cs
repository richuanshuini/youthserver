using FreeSql;
using YouthApartmentServer.Model.PropertyManagementModel;

namespace YouthApartmentServer.Repositories.IPropertyRepository;
public class PropertyRepository : BaseRepository<Property, int>, IPropertyRepository
{
    public PropertyRepository(IFreeSql fsql) : base(fsql)
    {
    }

    public ISelect<Property> Query() => Select.Where(p => !p.IsDeleted);
    public async Task<Property?> GetById(int id)
    {
        return await Select.Where(p=>p.PropertyId == id).FirstAsync();
    }
    
    public async Task<List<Property>> GetAllAsync()
    {
        return await Query().ToListAsync();
    }

    public async Task<Property> InseryAsync(Property property)
    {
        return await base.InsertAsync(property);
    }

    public async Task<(List<Property> Items,long Total)> GetPagedAsync(int pageNumber, int pageSize)
    {
        var query = Query();
        var total = await query.CountAsync();
        var items = await query.Page(pageNumber, pageSize).ToListAsync();
        return (items, total);
    }
}