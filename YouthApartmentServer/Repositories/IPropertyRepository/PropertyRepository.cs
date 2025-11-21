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

    public Task<List<Property>> InsertManyAsync(List<Property> properties)
    {
        return base.InsertAsync(properties);
    }

    public async Task<(List<Property> Items,long Total)> GetPagedAsync(int pageNumber, int pageSize)
    {
        var query = Query();
        var total = await query.CountAsync();
        var items = await query.Page(pageNumber, pageSize).ToListAsync();
        return (items, total);
    }
    
    //房源名称、地址、编号，仅通过Keyword传入
    public async Task<(List<Property> Items, long Total)> SearchAsync(PropertyQueryDto condition, int pageNumber, int pageSize)
    {
        var select = Query();

        var keyword = condition.Keyword?.Trim();
        if (!string.IsNullOrEmpty(keyword))
        {
            select = select.Where(p =>
                (p.PropertyName != null && p.PropertyName.Contains(keyword)) ||
                (p.Address != null && p.Address.Contains(keyword)) ||
                (p.PropertyCode != null && p.PropertyCode.Contains(keyword)));
        }

        if (condition.Status.HasValue)
        {
            select = select.Where(p => p.Status == condition.Status.Value);
        }

        if (condition.LeaseType.HasValue)
        {
            select = select.Where(p => p.LeaseType == condition.LeaseType.Value);
        }

        if (condition.LeaseTerm.HasValue)
        {
            select = select.Where(p => p.LeaseTerm == condition.LeaseTerm.Value);
        }

        if (condition.ApprovedByUser.HasValue)
        {
            select = select.Where(p => p.ApprovedByUser == condition.ApprovedByUser.Value);
        }

        if (condition.MinRentPrice.HasValue)
        {
            select = select.Where(p => p.RentPrice >= condition.MinRentPrice.Value);
        }

        if (condition.MaxRentPrice.HasValue)
        {
            select = select.Where(p => p.RentPrice <= condition.MaxRentPrice.Value);
        }

        if (condition.MinArea.HasValue)
        {
            select = select.Where(p => p.Area >= condition.MinArea.Value);
        }

        if (condition.MaxArea.HasValue)
        {
            select = select.Where(p => p.Area <= condition.MaxArea.Value);
        }

        if (condition.MinBedrooms.HasValue)
        {
            select = select.Where(p => p.Bedrooms >= condition.MinBedrooms.Value);
        }

        if (condition.MaxBedrooms.HasValue)
        {
            select = select.Where(p => p.Bedrooms <= condition.MaxBedrooms.Value);
        }

        if (condition.MinBathrooms.HasValue)
        {
            select = select.Where(p => p.Bathrooms >= condition.MinBathrooms.Value);
        }

        if (condition.MaxBathrooms.HasValue)
        {
            select = select.Where(p => p.Bathrooms <= condition.MaxBathrooms.Value);
        }

        if (condition.CreatedFrom.HasValue)
        {
            select = select.Where(p => p.CreatedAt >= condition.CreatedFrom.Value);
        }

        if (condition.CreatedTo.HasValue)
        {
            select = select.Where(p => p.CreatedAt <= condition.CreatedTo.Value);
        }

        if (condition.ApprovedFrom.HasValue)
        {
            select = select.Where(p => p.ApprovedAt >= condition.ApprovedFrom.Value);
        }

        if (condition.ApprovedTo.HasValue)
        {
            select = select.Where(p => p.ApprovedAt <= condition.ApprovedTo.Value);
        }

        if (condition.AvailableFrom.HasValue)
        {
            select = select.Where(p => p.AvailableDate >= condition.AvailableFrom.Value);
        }

        if (condition.AvailableTo.HasValue)
        {
            select = select.Where(p => p.AvailableDate <= condition.AvailableTo.Value);
        }

        var total = await select.CountAsync();
        var items = await select
            .OrderByDescending(p => p.CreatedAt)
            .Page(pageNumber, pageSize)
            .ToListAsync();

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
        updater.Set(a => a.UpdatedAt, property.UpdatedAt);
        
        var rows = await updater.ExecuteAffrowsAsync();
        return rows > 0;
    }
}
