using FreeSql;
using YouthApartmentServer.Model.PropertyManagementModel;

namespace YouthApartmentServer.Repositories.IAppointment;

public interface IAppointmentRepository
{
    public ISelect<Appointment> Query(); //查询没有被软删除的数据
    public Task<List<Appointment>> GetAllAsync();
    public Task<(List<Appointment> Item, long Total)> GetPagedAsync(int pageNumber, int pageSize);
}