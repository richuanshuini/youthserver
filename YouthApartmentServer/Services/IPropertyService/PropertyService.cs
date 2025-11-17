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
}