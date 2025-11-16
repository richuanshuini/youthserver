using FreeSql;
using YouthApartmentServer.Model.PropertyManagementModel;

namespace YouthApartmentServer.Repositories.IAppointment;

public class AppointmentRepository:BaseRepository<Appointment,int>,IAppointmentRepository
{
    public AppointmentRepository(IFreeSql fsql) : base(fsql) {}
    
    /// <summary>
    /// 查询没有被软删除的数据，由于应用了软删除，此处开始必须加上Query的返回值，才可以正确查询
    /// </summary>
    /// <returns></returns>
    public ISelect<Appointment> Query() => Select.Where(a => !a.IsDeleted);
    
    /// <summary>
    /// 按照创建时间排序返回数据
    /// </summary>
    /// <returns></returns>
    public async Task<List<Appointment>> GetAllAsync()
    {
        return await Query().OrderByDescending(a => a.CreatedAt).ToListAsync();
    }

    public async Task<(List<Appointment> Item, long Total)> GetPagedAsync(int pageNumber, int pageSize)
    {
        var total = await Select.CountAsync();
        var items = await Select.Page(pageNumber, pageSize).ToListAsync();
        return (items, total);
    }
}