using FreeSql;
using YouthApartmentServer.Model.PropertyManagementModel;
using YouthApartmentServer.ModelDto;

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

    public async Task<bool> UpdateAsync(int id, Property property)
    {
        var exist = await Query().Where(p => p.PropertyId == id).ToOneAsync();
        if(exist==null)
            return false;
        var updater = UpdateDiy.Where(p => p.PropertyId == id);
        
        if(property.RegionId!=null) 
            updater.Set(a=>a.RegionId,property.RegionId.Value);
        if(property.ApprovedByUser!=null)
            updater.Set(a=>a.ApprovedByUser,property.ApprovedByUser);
        if(property.Area!=null)
            updater.Set(a=>a.Area,property.Area.Value);
        if(property.Bedrooms!=null)
            updater.Set(a=>a.Bedrooms,property.Bedrooms.Value);
        if(property.Bathrooms!=null)
            updater.Set(a=>a.Bathrooms,property.Bathrooms.Value);
        if(property.MaxTenants!=null)
            updater.Set(a=>a.MaxTenants,property.MaxTenants.Value);
        if(property.PropertyName!=null)
            updater.Set(a=>a.PropertyName,property.PropertyName);
        if(property.Address!=null)
            updater.Set(a=>a.Address,property.Address);
        if(property.Description!=null)
            updater.Set(a=>a.Description,property.Description);
        if(property.PropertyCode!=null)
            updater.Set(a=>a.PropertyCode,property.PropertyCode);
        if(property.RoomNumber!=null)
            updater.Set(a=>a.RoomNumber,property.RoomNumber);
        if(property.RentPrice.HasValue)
            updater.Set(a=>a.RentPrice,property.RentPrice.Value);
        if (property.RentDeposit.HasValue)
            updater.Set(a => a.RentDeposit, property.RentDeposit.Value);
        if(property.PropertyFee.HasValue)
            updater.Set(a=>a.PropertyFee,property.PropertyFee.Value);
        if (property.Latitude.HasValue)
            updater.Set(a => a.Latitude, property.Latitude.Value);
        if (property.Longitude.HasValue)
            updater.Set(a => a.Longitude, property.Longitude.Value);
        if (property.LeaseType != exist.LeaseType)
            updater.Set(a => a.LeaseType, property.LeaseType);
        if (property.LeaseTerm != exist.LeaseTerm)
            updater.Set(a => a.LeaseTerm, property.LeaseTerm);
        if (property.AvailableDate.HasValue)
            updater.Set(a => a.AvailableDate, property.AvailableDate.Value);
        
        var rows = await updater.ExecuteAffrowsAsync();
        return rows > 0;
    }
}
