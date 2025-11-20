using Mapster;
using YouthApartmentServer.Model.PropertyManagementModel;
using YouthApartmentServer.ModelDto;
using YouthApartmentServer.Repositories.IPropertyRepository;
using YouthApartmentServer.Repositories.IUser;

namespace YouthApartmentServer.Services.IPropertyService;

public class PropertyService:IPropertyService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUserRepository _userRepository;

    public PropertyService(IPropertyRepository propertyRepository, IUserRepository userRepository)
    {
        _propertyRepository = propertyRepository;
        _userRepository = userRepository;
    }
    
    public async Task<List<Property>> GetAllPropertyAsync()
    {
        return await _propertyRepository.GetAllAsync();
    }
    
    public async Task<ValidationResult<Property>> CreatePropertyAsync(Property property)
    {
        var result =new ValidationResult<Property>();

        //检查审核员是否合法，由于_userRepository.GetByIdAsync传入的id是不可空类型的，但是确定这个参数不能为Null，因此对应可空类型，可以取Value+！来保证这个值不为NULL
        //因为是在控制器层从DTO映射过来的，因此必定不为NULL，DTO有[Request]检查
        var existuser = await _userRepository.GetByIdAsync(property.ApprovedByUser!.Value);
        if (existuser == null)
        {
            result.AddError("该审核员不存在，请重新输入");
        }
        
        //预留区域管理检查合法
        

        if (!result.IsValid)
            return result;
        
        //创建时间为初次创建时设置，其余的都是为NULL
        property.Status = PropertyStatus.PendingReview;
        property.CreatedAt=DateTime.Now;
        //最早可入住的时间点，为null时是随时可入住，跟CreateAt结合使用，例如创建于3月1号，那么有租客月底退房，最早入住时间是4月1号
        property.AvailableDate = null;
        property.UpdatedAt = null;
        property.ApprovedAt = null;
        property.IsDeleted=false;
        
        var entity= await _propertyRepository.InseryAsync(property);
        result.Data = entity;
        return result;
    }

    public async Task<PagedResult<Property>> GetPropertyPagedAsync(int pageNumber, int pageSize)
    {
        var (items,total)=await _propertyRepository.GetPagedAsync(pageNumber,pageSize);
        return new PagedResult<Property>()
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Total = total,
            Items = items
        };
    }

    public async Task<PagedResult<Property>> SearchPropertiesAsync(PropertyQueryDto query, int pageNumber, int pageSize)
    {
        var (items, total) = await _propertyRepository.SearchAsync(query, pageNumber, pageSize);
        return new PagedResult<Property>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Total = total,
            Items = items
        };
    }

    public async Task<ValidationResult<bool>> UpdatePropertyAsync(int id, Property property)
    {
        var result =new ValidationResult<bool>();
        //这里实现逻辑检查
        property.UpdatedAt=DateTime.Now;
        //在API接口层次拦截接口脏数据
        var exist = await _propertyRepository.GetById(id);
        if (exist == null)
            result.MarkNotFound("要更新的房源不存在");

        if (property.ApprovedByUser != null)
        {
            var existUser = await _userRepository.GetByIdAsync(property.ApprovedByUser.Value);
            if(existUser == null)
                result.AddError("分配的审核员不存在");
        }
        
        if(!result.IsValid)
            return result;
        //更新
        result.Data = await _propertyRepository.UpdateAsync(id, property);
        return result;
    }

    public async Task<ValidationResult<List<Property>>> BatchCreatePropertiesAsync(List<Property> properties)
    {
        var result = new ValidationResult<List<Property>>();
        if (properties.Count == 0)
        {
            result.AddError("至少需要提供一条房源记录");
            return result;
        }

        var cache = new Dictionary<int, bool>();
        var prepared = new List<Property>();
        var now = DateTime.Now;

        for (var i = 0; i < properties.Count; i++)
        {
            var entity = properties[i];
            if (!entity.ApprovedByUser.HasValue)
            {
                result.AddError($"第{i + 1}条数据缺少审核员ID");
                continue;
            }

            var approverId = entity.ApprovedByUser.Value;
            if (!cache.TryGetValue(approverId, out var exists))
            {
                var approver = await _userRepository.GetByIdAsync(approverId);
                exists = approver != null;
                cache[approverId] = exists;
            }

            if (!exists)
            {
                result.AddError($"第{i + 1}条数据的审核员ID {approverId} 不存在");
                continue;
            }

            entity.Status = PropertyStatus.PendingReview;
            entity.CreatedAt = now;
            entity.AvailableDate = null;
            entity.UpdatedAt = null;
            entity.ApprovedAt = null;
            entity.IsDeleted = false;
            prepared.Add(entity);
        }

        if (result.Errors.Count > 0)
        {
            return result;
        }

        var inserted = await _propertyRepository.InsertManyAsync(prepared);
        result.Data = inserted;
        return result;
    }
}
